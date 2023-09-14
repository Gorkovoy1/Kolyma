using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;
using UnityEngine.UI;

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

public class DialogueInk : MonoBehaviour
{
    public Image NPCPortrait;
    public TextAsset inkJSON;
    private Story inkStory;
    public TextMeshProUGUI dialogueText;
    public Button choiceButtonPrefab;
    public Transform choicesContainer;
    public bool startofDialogue;
    public bool choiceSelected;
    public bool isItalic;
    public bool isRed;
    public bool arkadyTalking;
    public bool NPCTalking;
    public float textSpeed;
    public TMP_FontAsset numbersFont;
    
    
    // Start is called before the first frame update
    void Start()
    {
        inkStory = new Story(inkJSON.text);
        StartCoroutine(ShowInkStory());
        startofDialogue = true;
    }

    IEnumerator ShowInkStory() 
    { int maxiterations = 999999;
        for (int i = 0; i < maxiterations; i++)
        {
            if (inkStory.canContinue)
            {
                if (Input.GetMouseButtonDown(0) || startofDialogue || choiceSelected)
                {
                    choiceSelected = false;
                    startofDialogue = false;
                    string text = inkStory.Continue();
                    textSpeed = 0.04f;
                    yield return StartCoroutine(LetterByLetter(text));
                }

            }
            else if (inkStory.currentChoices.Count > 0)
            {
                DisplayChoices();
                yield break;

            }
            yield return null;
        }   
    
    }
    IEnumerator LetterByLetter(string text) 
    {
        dialogueText.text = "";
        List<string> tags = inkStory.currentTags;
        if (tags.Count > 0)
        {if (tags[0] == "Narrator")
            {NPCPortrait.gameObject.SetActive (false);   
            }
        if (tags[0] == "Andreyev")
            {
                NPCPortrait.gameObject.SetActive(true);
            }
        }
        foreach (char letter in text)
        {
            if (letter == ' ')
            {
                isItalic = false;
                isRed = false;
                arkadyTalking = false;
                NPCTalking = false;
            }
            if (letter == '$')  //the word No is red lol - mark with something else and make it skip - maybe a ~ before Numbers. Note by SillyGoose 
            {
                isRed = true;
                continue;
            }
            if (letter == '<')
            {
                isItalic = true;
                continue;
            }
            if (letter == '@')                     // add these tags in ink
            {
                arkadyTalking = true;
                continue;
            }
            if (letter == '%')                     // add these tags in ink
            {
                NPCTalking = true;
                continue;
            }

            if (isItalic)
            {
                dialogueText.text += "<i>" + letter + "</i>";
            }
            else if (isRed)
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
    }
    void DisplayChoices()
    
    {foreach (Choice choice in inkStory.currentChoices)
        {
            Button choiceButton = Instantiate(choiceButtonPrefab, choicesContainer);
            choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
            choiceButton.onClick.AddListener(() => OnChoiceSelected(choice));
        }
    
    
    }
    void OnChoiceSelected(Choice choice) 
    {
        inkStory.ChooseChoiceIndex(choice.index);
        choicesContainer.ClearChildren();
        choiceSelected = true;
        StartCoroutine(ShowInkStory());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            textSpeed = 0.0006f;
        
        }
    }
}
