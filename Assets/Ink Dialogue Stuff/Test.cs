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

    public float textSpeed;
    public bool choiceSelected;
    public bool startOfDialogue;

    public bool isItalic;
    public bool isRed;
    public bool arkadyTalking;
    public bool NPCTalking;

    public Image NPC;
    public Image arkady;

    public TMP_FontAsset numbersFont;

    // Start is called before the first frame update
    private void Start()
    {
        inkStory = new Story(inkJSON.text);
        StartCoroutine(ShowInkStory());
        startOfDialogue = true;
    }

    IEnumerator ShowInkStory()
    {
        int maxIterations = 9999999; // Set a maximum number of iterations to prevent freezing

        for (int i = 0; i < maxIterations; i++)
        {
            if (inkStory.canContinue)
            {
                if(Input.GetMouseButtonDown(0) || choiceSelected || startOfDialogue)
                {
                    choiceSelected = false;
                    startOfDialogue = false;
                    string text = inkStory.Continue();
                    textSpeed = 0.04f;
                    yield return StartCoroutine(DisplayTextLetterByLetter(text));
                }
            }
            else if (inkStory.currentChoices.Count > 0)
            {
                DisplayChoices();
                yield break; // Wait for player input
            }

            yield return null; // Yield to the next frame
        }
    }

    IEnumerator DisplayTextLetterByLetter(string text)
    {
        dialogueText.text = ""; // Clear existing text

        foreach (char letter in text)
        {
            if(letter == ' ')
            {
                isItalic = false;
                isRed = false;
                arkadyTalking = false;
                NPCTalking = false;
            }
            if (letter == 'N')  //the word No is red lol - mark with something else and make it skip - maybe a ~ before Numbers. 
            {
                isRed = true;            
            }
            if(letter == '<')
            {
                isItalic = true;
                continue;
            }
            if(letter == '@')                     // add these tags in ink
            {
                arkadyTalking = true;
                continue;
            }
            if(letter == '%')                     // add these tags in ink
            {
                NPCTalking = true;
                continue;
            }

            if(isItalic)
            {
                dialogueText.text += "<i>" + letter + "</i>";
            }
            else if(isRed)
            {
                if (letter == '.' || letter == ',' || letter == '!' || letter == '?' || letter == ':')

                {
                    dialogueText.text += letter;


                }
                else
                {
                    dialogueText.text += "<color=red><font=\"" + numbersFont.name + "\">" + letter + "</font></color>";
                }
            }

            else
            {
                dialogueText.text += letter;
            }
            
            yield return new WaitForSeconds(textSpeed); // Adjust the delay between letters as needed
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
        choiceSelected = true;
        StartCoroutine(ShowInkStory());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Change the input condition as needed
        {
                textSpeed = 0.0006f;
        }

        if(arkadyTalking)
        {
            HighlightArkady();
        }

        if(NPCTalking)
        {
            HighlightNPC();
        }
    }

    void HighlightNPC()
    {
        Color colorNPC = NPC.color;
        Color colorArkady = arkady.color;
        colorNPC = new Color(colorNPC.r, colorNPC.g, colorNPC.b, 1f);
        colorArkady = new Color(colorArkady.r, colorArkady.g, colorArkady.b, 0.6f);
        NPC.color = colorNPC;
        arkady.color = colorArkady;
    }

    void HighlightArkady()
    {
        Color colorNPC = NPC.color;
        Color colorArkady = arkady.color;
        colorNPC = new Color(colorNPC.r, colorNPC.g, colorNPC.b, 0.6f);
        colorArkady = new Color(colorArkady.r, colorArkady.g, colorArkady.b, 1f);
        NPC.color = colorNPC;
        arkady.color = colorArkady;
    }

}