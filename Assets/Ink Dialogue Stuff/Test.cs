using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
using TMPro;

public static class TransformExtensions
{
    // Extension method to clear the children of a Transform
    public static void ClearChildren(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}

public class Test : MonoBehaviour
{
    public TextAsset inkJSON;
    private Story inkStory;

    public TextMeshProUGUI dialogueText;
    public Button choiceButtonPrefab;
    public Transform choicesContainer;

    // Start is called before the first frame update
    private void Start()
    {
        inkStory = new Story(inkJSON.text);
        StartCoroutine(ShowInkStory());
    }

    IEnumerator ShowInkStory()
    {
        while (inkStory.canContinue || inkStory.currentChoices.Count > 0)
        {
            if (inkStory.canContinue)
            {
                string text = inkStory.Continue();
                yield return StartCoroutine(DisplayTextLetterByLetter(text));
            }
            else if (inkStory.currentChoices.Count > 0)
            {
                DisplayChoices();
                yield break; // Wait for player input
            }
        }
    }

    IEnumerator DisplayTextLetterByLetter(string text)
    {
        dialogueText.text = ""; // Clear existing text

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f); // Adjust the delay between letters as needed
        }

        // Optional: You can wait for player input or a timer before continuing.
        // For example, you can use Input.GetKeyDown or a timer to wait for player input.
        // After the delay, you can call another function or continue the story.
    }


    void DisplayChoices()
    {
        foreach (Choice choice in inkStory.currentChoices)
        {
            Button choiceButton = Instantiate(choiceButtonPrefab, choicesContainer);
            choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
            choiceButton.onClick.AddListener(() => OnChoiceSelected(choice));
        }
    }

    void OnChoiceSelected(Choice choice)
    {
        inkStory.ChooseChoiceIndex(choice.index);
        choicesContainer.ClearChildren(); // Clear the choice buttons
        StartCoroutine(ShowInkStory());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Change the input condition as needed
        {
            ShowInkStory();
        }
    }

}
