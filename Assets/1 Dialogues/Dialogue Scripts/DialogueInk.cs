using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;
using UnityEngine.UI;
using System;

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
    public Image PlayerPortrait;
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

    public const string Dissolve = "_Dissolve";

    public GameObject ambientObj;
    public GameObject levelLoader;
    public bool playSound;
    public bool soundEnded;
    public bool startScene;
    public AK.Wwise.Event soundEvent;
    

    // Start is called before the first frame update
    void Start()
    {
        inkStory = new Story(inkJSON.text);
        GameObject levelLoader = GameObject.Find("LevelLoader");
        PlayerPortrait.gameObject.SetActive(false);
        NPCPortrait.gameObject.SetActive(false);
        startScene = true;
        StartCoroutine(ShowInkStory());
        startofDialogue = true;
        soundEnded = true;
        playSound = false;
        ambientObj.SetActive(false);
    }

    IEnumerator ShowInkStory()
    {
        if(startScene)
        {
            yield return new WaitForSeconds(6f);
            ambientObj.SetActive(true);
            AkSoundEngine.PostEvent("Play_Music_Jail", gameObject);
            Debug.Log("Music");
            
            yield return new WaitForSeconds(3f);
        }
        

        startScene = false;

        int maxiterations = 999999;
        for (int i = 0; i < maxiterations; i++)
        {
            if (inkStory.canContinue)
            {
                if (choiceSelected || Input.GetMouseButtonDown(0) || startofDialogue)
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
                //choiceSelected = true;
                yield break;

            }
            yield return null;
        }

    }
    IEnumerator LetterByLetter(string text)
    {
        dialogueText.text = "";
        //Debug.Log(text);
        List<string> tags = inkStory.currentTags;
        if(text == "\n")
        {
            choiceSelected = true;
            HighlightNPC();
        }
        else
        {
            if (tags.Count > 0)
            {
                if (tags[0] == "SetPortraitsActive")
                {
                    NPCPortrait.gameObject.SetActive(true);
                    PlayerPortrait.gameObject.SetActive(true);
                    //NPCPortrait.GetComponent<Material>().SetFloat(Dissolve, -0.6f);
                    //NPCPortrait.GetComponent<Material>().SetFloat(Dissolve, 0f);
                    //NPCPortrait.GetComponent<Material>().SetFloat(Dissolve, 0.5f);
                    HighlightNPC();

                }



                if (tags[0] == "Narrator")
                {
                    NPCPortrait.gameObject.SetActive(false);
                    PlayerPortrait.gameObject.SetActive(false);
                }
                if (tags[0] == "Andreyev")
                {
                    HighlightNPC();
                }
                if (tags[0] == "Arkady")
                {
                    HighlightPlayer();

                }
            }

            if(tags[0] != "Narrator")
            {
                Debug.Log("foreach letter");
                foreach (char letter in text)
                {
                    playSound = true;
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
            else
            {
                //its intro
                dialogueText.text = text;
                //make alpha increase with time
                dialogueText.alpha = 0f;

                while(dialogueText.alpha < 1f)
                {
                    dialogueText.alpha += 0.3f * Time.deltaTime;
                    yield return null;
                }
            }
            playSound = false;
            
        }
        
    }

    public void PlaySound() 
    {
        soundEvent.Post(gameObject, (uint)AkCallbackType.AK_EndOfEvent, SoundEndedCallback);
    }

    private void SoundEndedCallback(object in_cookie, AkCallbackType in_type, object in_info)
    {
        // Check if the callback type is EndOfEvent, indicating the sound has ended
        if (in_type == AkCallbackType.AK_EndOfEvent)
        {
            Debug.Log("Sound has ended.");
            soundEnded = true;
        }
    }


    void DisplayChoices()

    {
        HighlightPlayer();

        foreach (Choice choice in inkStory.currentChoices)
        {
            Button choiceButton = Instantiate(choiceButtonPrefab, choicesContainer);
            choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
            choiceButton.onClick.AddListener(() => OnChoiceSelected(choice));
        }
    }

    void OnChoiceSelected(Choice choice)
    {
        choiceSelected = true;
        inkStory.ChooseChoiceIndex(choice.index);
        choicesContainer.ClearChildren();
        StartCoroutine(ShowInkStory());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !startScene)
        {
            textSpeed = 0.0006f;
            dialogueText.alpha = 1f;
        }
    }
    void HighlightNPC()
    {
        Color colorNPC = NPCPortrait.color;
        Color colorArkady = PlayerPortrait.color;
        colorNPC = new Color(colorNPC.r, colorNPC.g, colorNPC.b, 1f);
        colorArkady = new Color(colorArkady.r, colorArkady.g, colorArkady.b, 0.6f);
        NPCPortrait.color = colorNPC;
        PlayerPortrait.color = colorArkady;
    }

    void HighlightPlayer()
    {
        Color colorNPC = NPCPortrait.color;
        Color colorArkady = PlayerPortrait.color;
        colorNPC = new Color(colorNPC.r, colorNPC.g, colorNPC.b, 0.6f);
        colorArkady = new Color(colorArkady.r, colorArkady.g, colorArkady.b, 1f);
        NPCPortrait.color = colorNPC;
        PlayerPortrait.color = colorArkady;

    }
}
