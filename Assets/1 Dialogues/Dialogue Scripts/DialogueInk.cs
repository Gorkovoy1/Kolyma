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
    public GameObject NPCPortrait;
    public GameObject PlayerPortrait;
    

    public TextAsset inkJSON;
    private Story inkStory;

    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI introText;
    public TextMeshProUGUI nameTag;
    public GameObject narratorTag;
    public Image paper;
    public Color paperColor;
    public Color tagColor;
    public TMP_FontAsset numbersFont;

    public Button choiceButtonPrefab;
    public Transform choicesContainer;

    public bool startofDialogue;
    public bool choiceSelected;
    public bool isItalic;
    public bool isRed;
    public bool arkadyTalking;
    public bool NPCTalking;
    public bool playSound;
    public bool soundEnded;
    public bool startScene;
    public bool skip;

    public float textSpeed;

    public const string Dissolve = "_Dissolve";

    public GameObject ambientObj;
    public GameObject levelLoader;

    public AK.Wwise.Event soundEvent;

    public List<string> tags;

    public GameObject smoke;

    

    //setting up scene objects
    public int dialogueNumber;
    public Image background;
    public Sprite bgd1;
    public Sprite bgd2;
    public string musicName;
    public TextAsset inkd1;
    public TextAsset inkd2;
    public SpriteRenderer NPCBlack;
    public SpriteRenderer NPCColor;
    public Sprite andreyevBlack;
    public Sprite andreyevColor;
    public Sprite rybakovBlack;
    public Sprite rybakovColor;
    public TextMeshProUGUI loaderText;
   
    void Start()
    {
        //retrieve which dialogue this is and set things up
        //dialogueNumber = SceneData.DialogueNumber;

        DialogueSetUp();

        //create ink file and set level loader. deactivate everything while levelloader is active, reset bools and set paper to invisible
        inkStory = new Story(inkJSON.text);
        GameObject levelLoader = GameObject.Find("LevelLoader");
        introText.text = "";
        PlayerPortrait.gameObject.SetActive(false);
        NPCPortrait.gameObject.SetActive(false);
        startScene = true;
        StartCoroutine(ShowInkStory());
        startofDialogue = true;
        soundEnded = true;
        playSound = false;
        ambientObj.SetActive(false);
        skip = false;
        paperColor = paper.color;
        paper.color = new Color(paperColor.r, paperColor.g, paperColor.b, 0f);
        narratorTag.gameObject.SetActive(false);

        //dialogue1
        if(dialogueNumber == 1){
            nameTag.text = "Old Man";
        }
        

        //change this object and position for each scene
        smoke.gameObject.SetActive(false);



    }

    void DialogueSetUp()
    {
        if(dialogueNumber == 1)
        {
            //set background
            background.sprite = bgd1;
            //set NPCPortrait
            NPCBlack.sprite = andreyevBlack;
            NPCColor.sprite = andreyevColor;
            //set smoke/background animations
            //set loader text
            loaderText.text = "Wednesday, November 30th, 1938" + 
            Environment.NewLine + Environment.NewLine + 
            "Magadan, City Jail, The Soviet Union";
            //set Music
            musicName = "Play_Music_Jail";
            //set ambient sounds
            //set opening sound (jail door)
            //set ink file
            inkJSON = inkd1;
            //set next scene to load

        }

        else if(dialogueNumber == 2)
        {
            //set background
            background.sprite = bgd2;
            //set NPCPortrait
            NPCBlack.sprite = rybakovBlack;
            NPCColor.sprite = rybakovColor;
            //set smoke/background animations
            //set loader text
            loaderText.text = "New Text Here";
            //set Music
            //set ambient sounds
            //set opening sound (jail door)
            //set ink file
            inkJSON = inkd2;
            //set next scene to load
        }
    }

    IEnumerator ShowInkStory()
    {       
        //dont start the dialogue until levelloader is gone, then start sounds and dialogue
        if(startScene)
        {
            yield return new WaitForSeconds(6f);
            ambientObj.SetActive(true);
            AkSoundEngine.PostEvent(musicName, gameObject);
            Debug.Log("Music");

            //pause as we look at the background
            yield return new WaitForSeconds(3f);


            smoke.gameObject.SetActive(true);
            
            
            yield return new WaitForSeconds(4f);
            
            //fade paper in
            narratorTag.gameObject.SetActive(true);
            AkSoundEngine.PostEvent("Play_Woosh_Narrator", gameObject);
            while(paper.color.a < 1f)
            {
                paper.color = new Color(paperColor.r, paperColor.g, paperColor.b, paper.color.a + 0.3f * Time.deltaTime);
                yield return null;
            }
            paper.color = new Color(paperColor.r, paperColor.g, paperColor.b, 1f);
            
        }
        
        //no longer intro
        startScene = false;
        introText.text = "";

        //for each line in ink, show it letter by letter unless its a choice 
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

    //display each line letter by letter
    IEnumerator LetterByLetter(string text)
    {
        dialogueText.text = "";
        tags = inkStory.currentTags;
        //Debug.Log(text);
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
                    introText.text = "";
                    HighlightNPC();
                }

                if(tags[0] == "ChangeName")
                {
                    nameTag.text = "Andreyev";
                }

                if (tags[0] == "Narrator")
                {
                    NPCPortrait.gameObject.SetActive(false);
                    PlayerPortrait.gameObject.SetActive(false);
                    narratorTag.gameObject.SetActive(true);
                }
                if (tags[0] == "NarratorSound")
                {
                    NPCPortrait.gameObject.SetActive(false);
                    AkSoundEngine.PostEvent("Play_Woosh_Narrator",gameObject);
                    PlayerPortrait.gameObject.SetActive(false);
                    narratorTag.gameObject.SetActive(true);
                }
                if (tags[0] == "Andreyev")
                {
                    HighlightNPC();
                    narratorTag.gameObject.SetActive(false);
                }
                if (tags[0] == "Arkady")
                {
                    HighlightPlayer();
                    narratorTag.gameObject.SetActive(false);
                }
            }

            if (tags[0] != "Narrator" && tags[0] != "NarratorSound")
            {
                Debug.Log("foreach letter");
                introText.text = "";
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
                        playSound = true;
                        dialogueText.text += letter;
                    }
               
                    if(!skip)
                    {
                        yield return new WaitForSeconds(textSpeed); // Adjust the delay between letters as needed 
                    }
                }
                
                skip = false;
            }
            else
            {
               
                //its intro
                introText.text = text;
                //make alpha increase with time
                introText.alpha = 0f;

                while(introText.alpha < 1f)
                {
                    introText.alpha += 0.3f * Time.deltaTime;
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
        AkSoundEngine.PostEvent("Play_Click", gameObject);
        inkStory.ChooseChoiceIndex(choice.index);
        choicesContainer.ClearChildren();
        StartCoroutine(ShowInkStory());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !startScene)
        {
            //textSpeed = 0.0006f;
            introText.alpha = 1f;
            if(playSound)
            {
                skip = true;
            }
        }

        if (playSound && soundEnded)
        {
            soundEnded = false;
            PlaySound();
        }

    }
    void HighlightNPC()
    {
        narratorTag.gameObject.SetActive(false);
        NPCPortrait.gameObject.SetActive(true);
        PlayerPortrait.gameObject.SetActive(false);
    }

    void HighlightPlayer()
    {
        narratorTag.gameObject.SetActive(false);
        PlayerPortrait.gameObject.SetActive(true);
        NPCPortrait.gameObject.SetActive(false);

    }
}
