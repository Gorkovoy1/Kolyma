using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

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
    public Color backgroundColor;
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
    public bool isSaved = false;
    public bool pause = false;

    public float textSpeed;

    public const string Dissolve = "_Dissolve";

    public GameObject ambientObj;
    public GameObject levelLoaderParent;

    public AK.Wwise.Event soundEvent;

    public List<string> tags;

    public GameObject smoke;

    public GameObject darkPrefab;

    public GameObject bedObj;
    

    //setting up scene objects
    public int dialogueNumber;
    public Image background;
    public string musicName;
    public SpriteRenderer NPCBlack;
    public SpriteRenderer NPCColor;
    public TextMeshProUGUI loaderText;
    public GameObject backgroundAnimParent;
    public GameObject screenSFXParent;
    public GameObject ambientParent;

    public DialogueScriptableObject dialogueObj1;
    public DialogueScriptableObject dialogueObj2;
    public DialogueScriptableObject dialogueObj3;
    public DialogueScriptableObject dialogueObj4;
    public DialogueScriptableObject dialogueObj5;
    public DialogueScriptableObject dialogueObj6;

    public int nextSceneNumber;
   
    void Start()
    {
        //retrieve which dialogue this is and set things up
        //dialogueNumber = SceneData.DialogueNumber;

        //load assets depending on scriptable object
        DialogueSetUp(); 
        screenSFXParent.gameObject.SetActive(true);

        if (PlayerPrefs.HasKey("SavedInkState"))
        { 
            Debug.Log("hasplayerprefs");
            inkStory = new Story(inkJSON.text);
            string savedState = PlayerPrefs.GetString("SavedInkState");
            inkStory.state.LoadJson(savedState);
            GameObject levelLoaderParent = GameObject.Find("LevelLoaderParent");
            levelLoaderParent.gameObject.SetActive(false);
            screenSFXParent.gameObject.SetActive(false);
            introText.text = "";
            PlayerPortrait.gameObject.SetActive(false);
            NPCPortrait.gameObject.SetActive(false);
            StartCoroutine(ShowInkStory());
            //not sure if true or false
            startScene = false;
            startofDialogue = true;
            soundEnded = true;
            playSound = false;
            ambientObj.SetActive(true);
            Debug.Log("first ambient");
            AkSoundEngine.PostEvent(musicName, gameObject);
            skip = false;
            paperColor = paper.color;
            paper.color = new Color(paperColor.r, paperColor.g, paperColor.b, 1f);
            narratorTag.gameObject.SetActive(false);
            //??? change?
            smoke = backgroundAnimParent;
            smoke.gameObject.SetActive(true);
            isSaved = true;
        }

        else
        {
            //create ink file and set level loader. deactivate everything while levelloader is active, reset bools and set paper to invisible
            inkStory = new Story(inkJSON.text);
            GameObject levelLoaderParent = GameObject.Find("LevelLoaderParent");
            levelLoaderParent.gameObject.SetActive(true);
            introText.text = "";
            PlayerPortrait.gameObject.SetActive(false);
            NPCPortrait.gameObject.SetActive(false);
            bedObj.gameObject.SetActive(false);
            startScene = true;
            ambientObj.SetActive(false);
            Debug.Log("Ambient Off");
            StartCoroutine(ShowInkStory());
            startofDialogue = true;
            soundEnded = true;
            playSound = false;
            skip = false;
            paperColor = paper.color;
            paper.color = new Color(paperColor.r, paperColor.g, paperColor.b, 0f);
            narratorTag.gameObject.SetActive(false);

            //dialogue1
            if(dialogueNumber == 1){
                nameTag.text = "Old Man";
            }
        

            //change this object and position for each scene
            smoke = backgroundAnimParent;
            smoke.gameObject.SetActive(false);
        }

        PlayerPrefs.DeleteKey("SavedInkState");


    }

    void DialogueSetUp()
    {
        if(dialogueNumber != 3)
        {
            bedObj.SetActive(false);
        }

        if(dialogueNumber == 1)
        {
            //set level loader and text
            foreach (Transform child in levelLoaderParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newLoader = Instantiate(dialogueObj1.blackScreen, levelLoaderParent.transform);
            if(dialogueObj1.line1 != "" && dialogueObj1.line2 != "")
            {
                newLoader.GetComponentInChildren<TextMeshProUGUI>().text = dialogueObj1.line1 + Environment.NewLine + Environment.NewLine + dialogueObj1.line2;
            }
            //set background
            background.sprite = dialogueObj1.bg;
            //set NPCPortrait
            NPCBlack.sprite = dialogueObj1.npcBlack;
            NPCColor.sprite = dialogueObj1.npcColor;
            //set NPC nameTag
            nameTag.text = dialogueObj1.npcName;
            //set smoke/background animations
            foreach(Transform child in backgroundAnimParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newBgAnim = Instantiate(dialogueObj1.bganim, backgroundAnimParent.transform);
            //set Music
            musicName = dialogueObj1.music;
            //set ambient sounds, delete all children and instantiate teh correct new prefab 
            foreach (Transform child in ambientParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newAmbient = Instantiate(dialogueObj1.ambient, ambientParent.transform);
            //set opening sound (jail door sfx)
            foreach(Transform child in screenSFXParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newSFX = Instantiate(dialogueObj1.sfx, screenSFXParent.transform);
            //set ink file
            inkJSON = dialogueObj1.inkfile;
            //set next scene to load
            nextSceneNumber = dialogueObj1.nextScene;
        }
        
        else if (dialogueNumber == 2)
        {
            //set level loader
            foreach (Transform child in levelLoaderParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newLoader = Instantiate(dialogueObj2.blackScreen, levelLoaderParent.transform);
            if(dialogueObj2.line1 != "" && dialogueObj2.line2 != "")
            {
                newLoader.GetComponentInChildren<TextMeshProUGUI>().text = dialogueObj2.line1 + Environment.NewLine + Environment.NewLine + dialogueObj2.line2;
            }
            //set background
            background.sprite = dialogueObj2.bg;
            //set NPCPortrait
            NPCBlack.sprite = dialogueObj2.npcBlack;
            NPCColor.sprite = dialogueObj2.npcColor;
            //set NPC nameTag
            nameTag.text = dialogueObj2.npcName;
            //set smoke/background animations
            foreach(Transform child in backgroundAnimParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newBgAnim = Instantiate(dialogueObj2.bganim, backgroundAnimParent.transform);
            //set Music
            musicName = dialogueObj2.music;
            //set ambient sounds, delete all children and instantiate teh correct new prefab 
            foreach (Transform child in ambientParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newAmbient = Instantiate(dialogueObj2.ambient, ambientParent.transform);
            //set opening sound (jail door sfx)
            foreach(Transform child in screenSFXParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newSFX = Instantiate(dialogueObj2.sfx, screenSFXParent.transform);
            //set ink file
            inkJSON = dialogueObj2.inkfile;
            //set next scene to load
            nextSceneNumber = dialogueObj2.nextScene;
        }

        else if (dialogueNumber == 3)
        {
            //set level loader
            foreach (Transform child in levelLoaderParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newLoader = Instantiate(dialogueObj3.blackScreen, levelLoaderParent.transform);
            if(dialogueObj3.line1 != "" && dialogueObj3.line2 != "")
            {
                newLoader.GetComponentInChildren<TextMeshProUGUI>().text = dialogueObj3.line1 + Environment.NewLine + Environment.NewLine + dialogueObj3.line2;
            }
            //set background
            background.sprite = dialogueObj3.bg;
            //set NPCPortrait
            NPCBlack.sprite = dialogueObj3.npcBlack;
            NPCColor.sprite = dialogueObj3.npcColor;
            //set NPC nameTag
            nameTag.text = dialogueObj3.npcName;
            //set smoke/background animations
            foreach(Transform child in backgroundAnimParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newBgAnim = Instantiate(dialogueObj3.bganim, backgroundAnimParent.transform);
            //set Music
            musicName = dialogueObj3.music;
            //set ambient sounds, delete all children and instantiate teh correct new prefab 
            foreach (Transform child in ambientParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newAmbient = Instantiate(dialogueObj3.ambient, ambientParent.transform);
            //set opening sound (jail door sfx)
            foreach(Transform child in screenSFXParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newSFX = Instantiate(dialogueObj3.sfx, screenSFXParent.transform);
            //set ink file
            inkJSON = dialogueObj3.inkfile;
            //set next scene to load
            nextSceneNumber = dialogueObj3.nextScene;
        }
        else if (dialogueNumber == 4)
        {
            //set level loader and text
            foreach (Transform child in levelLoaderParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newLoader = Instantiate(dialogueObj4.blackScreen, levelLoaderParent.transform);
            if (dialogueObj4.line1 != "" && dialogueObj4.line2 != "")
            {
                newLoader.GetComponentInChildren<TextMeshProUGUI>().text = dialogueObj4.line1 + Environment.NewLine + Environment.NewLine + dialogueObj4.line2;
            }
            //set background
            background.sprite = dialogueObj4.bg;
            //set NPCPortrait
            NPCBlack.sprite = dialogueObj4.npcBlack;
            NPCColor.sprite = dialogueObj4.npcColor;
            //set NPC nameTag
            nameTag.text = dialogueObj4.npcName;
            //set smoke/background animations
            foreach (Transform child in backgroundAnimParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newBgAnim = Instantiate(dialogueObj4.bganim, backgroundAnimParent.transform);
            //set Music
            musicName = dialogueObj4.music;
            //set ambient sounds, delete all children and instantiate teh correct new prefab 
            foreach (Transform child in ambientParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newAmbient = Instantiate(dialogueObj4.ambient, ambientParent.transform);
            //set opening sound (jail door sfx)
            foreach (Transform child in screenSFXParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newSFX = Instantiate(dialogueObj4.sfx, screenSFXParent.transform);
            //set ink file
            inkJSON = dialogueObj4.inkfile;
            //set next scene to load
            nextSceneNumber = dialogueObj4.nextScene;
        }
        else if (dialogueNumber == 5)
        {
            //set level loader and text
            foreach (Transform child in levelLoaderParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newLoader = Instantiate(dialogueObj5.blackScreen, levelLoaderParent.transform);
            if (dialogueObj5.line1 != "" && dialogueObj5.line2 != "")
            {
                newLoader.GetComponentInChildren<TextMeshProUGUI>().text = dialogueObj5.line1 + Environment.NewLine + Environment.NewLine + dialogueObj5.line2;
            }
            //set background
            background.sprite = dialogueObj5.bg;
            //set NPCPortrait
            NPCBlack.sprite = dialogueObj5.npcBlack;
            NPCColor.sprite = dialogueObj5.npcColor;
            //set NPC nameTag
            nameTag.text = dialogueObj5.npcName;
            //set smoke/background animations
            foreach (Transform child in backgroundAnimParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newBgAnim = Instantiate(dialogueObj5.bganim, backgroundAnimParent.transform);
            //set Music
            musicName = dialogueObj5.music;
            //set ambient sounds, delete all children and instantiate teh correct new prefab 
            foreach (Transform child in ambientParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newAmbient = Instantiate(dialogueObj5.ambient, ambientParent.transform);
            //set opening sound (jail door sfx)
            foreach (Transform child in screenSFXParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newSFX = Instantiate(dialogueObj5.sfx, screenSFXParent.transform);
            //set ink file
            inkJSON = dialogueObj5.inkfile;
            //set next scene to load
            nextSceneNumber = dialogueObj5.nextScene;
        }
        else if (dialogueNumber == 6)
        {
            //set level loader and text
            foreach (Transform child in levelLoaderParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newLoader = Instantiate(dialogueObj6.blackScreen, levelLoaderParent.transform);
            if (dialogueObj6.line1 != "" && dialogueObj6.line2 != "")
            {
                newLoader.GetComponentInChildren<TextMeshProUGUI>().text = dialogueObj6.line1 + Environment.NewLine + Environment.NewLine + dialogueObj6.line2;
            }
            //set background
            background.sprite = dialogueObj6.bg;
            //set NPCPortrait
            NPCBlack.sprite = dialogueObj6.npcBlack;
            NPCColor.sprite = dialogueObj6.npcColor;
            //set NPC nameTag
            nameTag.text = dialogueObj6.npcName;
            //set smoke/background animations
            foreach (Transform child in backgroundAnimParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newBgAnim = Instantiate(dialogueObj6.bganim, backgroundAnimParent.transform);
            //set Music
            musicName = dialogueObj6.music;
            //set ambient sounds, delete all children and instantiate teh correct new prefab 
            foreach (Transform child in ambientParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newAmbient = Instantiate(dialogueObj6.ambient, ambientParent.transform);
            //set opening sound (jail door sfx)
            foreach (Transform child in screenSFXParent.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newSFX = Instantiate(dialogueObj6.sfx, screenSFXParent.transform);
            //set ink file
            inkJSON = dialogueObj6.inkfile;
            //set next scene to load
            nextSceneNumber = dialogueObj6.nextScene;
        }
    }

    IEnumerator ShowInkStory()
    {
        if (pause)
        {
            PauseScene();
        }
        //dont start the dialogue until levelloader is gone, then start sounds and dialogue
        else if (startScene)
        {
            
                if (dialogueNumber != 3)
                {
                    yield return new WaitForSeconds(6f);
                }
                else
                {
                    yield return new WaitForSeconds(2f);
                }


                ambientObj.SetActive(true);
                Debug.Log("second ambient");
                AkSoundEngine.PostEvent(musicName, gameObject);
                Debug.Log("Music");

                //Debug.Log("ambient active");

                //pause as we look at the background
                if (dialogueNumber != 3)
                { yield return new WaitForSeconds(3f); }
                else { yield return new WaitForSeconds(1f); }


                smoke.gameObject.SetActive(true);


                yield return new WaitForSeconds(4f);

                //fade paper in
                //narratorTag.gameObject.SetActive(true);
                AkSoundEngine.PostEvent("Play_Woosh_Narrator", gameObject);
                while (paper.color.a < 1f)
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

    //display each line letter by letter. TAGS ARE HERE!
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
                    AkSoundEngine.PostEvent("Play_Andreyev_Clears_Throat", gameObject);
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
                    NarratorSound();
                }
                if (tags[0] == "Andreyev" || tags[0] == "Rybakov"|| tags[0] == "Gangster" || tags[0] == "GenericPrisoner" || tags[0] == "Boris")
                {
                    HighlightNPC();
                    narratorTag.gameObject.SetActive(false);
                }
                if (tags[0] == "Arkady")
                {
                    HighlightPlayer();
                    narratorTag.gameObject.SetActive(false);
                }
                if (tags[0] == "Tutorial")
                {
                    HighlightNPC();
                    narratorTag.gameObject.SetActive(false);

                    pause = true;

                }
                //andreyev utterances
                if (tags[0] == "Andreyev_Clears_Throat")
                {
                    HighlightNPC();
                    narratorTag.gameObject.SetActive(false);
                    AkSoundEngine.PostEvent("Play_Andreyev_Clears_Throat", gameObject);
                }
                if (tags[0] == "Andreyev_Satisfied")
                {
                    HighlightNPC();
                    narratorTag.gameObject.SetActive(false);
                    AkSoundEngine.PostEvent("Play_Andreyev_Satisfied", gameObject);
                }
                if (tags[0] == "Andreyev_Scared")
                {
                    HighlightNPC();
                    narratorTag.gameObject.SetActive(false);
                    AkSoundEngine.PostEvent("Play_Andreyev_Scared", gameObject);
                }
                if (tags[0] == "Andreyev_Good_Luck")
                {
                    HighlightNPC();
                    narratorTag.gameObject.SetActive(false);
                    AkSoundEngine.PostEvent("Play_Andreyev_Good_Luck", gameObject);
                }
                if (tags[0] == "Andreyev_Sigh")
                {
                    HighlightNPC();
                    narratorTag.gameObject.SetActive(false);
                    AkSoundEngine.PostEvent("Play_Andreyev_Sigh", gameObject);
                }
                if (tags[0] == "Andreyev_Snarky")
                {
                    HighlightNPC();
                    narratorTag.gameObject.SetActive(false);
                    AkSoundEngine.PostEvent("Play_Andreyev_Snarky", gameObject);
                }
                if (tags[0] == "Andreyev_Question")
                {
                    HighlightNPC();
                    narratorTag.gameObject.SetActive(false);
                    AkSoundEngine.PostEvent("Play_Andreyev_Question", gameObject);
                }
                //end of utterances
                if (tags[0] == "NegativeEvent")
                {
                    HighlightNPC();
                    narratorTag.gameObject.SetActive(false);
                    AkSoundEngine.PostEvent("Play_Negative_Event", gameObject); 
                }
                if (tags[0] == "ReceiveItem")
                {
                    NarratorSound();

                    //this executes before the line finishes
                    //LoadNextScene();
                }

                if(tags[0] == "ClickBed" || tags[0] == "ClickDoor") //bedobj is just any clickable obj
                {
                    NarratorSound();

                    
                    bedObj.gameObject.SetActive(true);

                }

                if(tags[0] == "Dark")
                {
                    //instantiate black screen with fadeout, control black screen seconds and fadeout speed
                    GameObject dark = Instantiate(darkPrefab, this.transform);
                    NPCPortrait.gameObject.SetActive(false);
                    backgroundAnimParent.gameObject.SetActive(false);
                    dark.GetComponent<Dark>().duration = 1f;
                    dark.GetComponent<Dark>().speed = 0.6f;
                    AkSoundEngine.PostEvent("Play_Punch", gameObject);
                    while(dark != null)
                    {
                        yield return null;
                    }
                    NPCPortrait.gameObject.SetActive(true);
                    backgroundAnimParent.gameObject.SetActive(true);
                }

                if (tags[0] == "CloseEyes")
                {
                    Debug.Log("line1");
                    //set background to black
                    StartCoroutine(FadeBackground());
                    //background.color = new Color(0f, 0f, 0f, 1f);
                    NPCPortrait.gameObject.SetActive(false);
                }

                if (tags[0] == "ConvoyOfficer")
                {
                    //set npc to convoy officer - change this to right image
                    NPCBlack.sprite = dialogueObj2.npcBlack;
                    NPCColor.sprite = dialogueObj2.npcColor;
                    nameTag.text = "Convoy Officer";

                    HighlightNPC();
                    AkSoundEngine.PostEvent("Play_Cell_Guard_Roll_Call", gameObject);
                }
                
                if (tags[0] == "TakeAway")
                {
                    NPCPortrait.gameObject.SetActive(false);
                    //set npc back to andreyev
                    NPCBlack.sprite = dialogueObj1.npcBlack;
                    NPCColor.sprite = dialogueObj1.npcColor;
                    nameTag.text = dialogueObj1.npcName;
                    AkSoundEngine.PostEvent("Play_Cell_Door_Open", gameObject);
                    AkSoundEngine.PostEvent("Play_Andreyev_Scared", gameObject);

                    StartCoroutine(FadeBackColor());
                    HighlightNPC();
                    
                }
            }

            if (tags[0] != "Narrator" && tags[0] != "NarratorSound" && tags[0] != "CloseEyes")
            {
                Debug.Log("foreach letter");
                introText.text = "";
                foreach (char letter in text)
                {
                    playSound = true;
                    if (letter == ' ' || letter == '.')
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
                //this is a test
                if(tags[0] == "ReceiveItem"){
                        StartCoroutine(LoadNextScene());
                }
                if (tags[0] == "DialogueEnd")
                {
                    StartCoroutine(LoadNextScene());
                }
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

        //pause = true;
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

    void PauseScene()
    {
        string savedState = inkStory.state.ToJson();
        PlayerPrefs.SetString("SavedInkState", savedState);
        AkSoundEngine.StopAll();
        SceneManager.LoadScene("2 TutorialScene");
    }

    //end of dialogue
    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(1f);
        PlayerPrefs.DeleteKey("SavedInkState");
        AkSoundEngine.StopAll();
        SceneManager.LoadScene(nextSceneNumber);

        //fade out sounds with rtpc? 
    }

    void NarratorSound()
    {
        NPCPortrait.gameObject.SetActive(false);
        AkSoundEngine.PostEvent("Play_Woosh_Narrator",gameObject);
        PlayerPortrait.gameObject.SetActive(false);
        narratorTag.gameObject.SetActive(true);
    }

    private IEnumerator FadeBackColor()
    {
        Color originalColor = backgroundColor; // Store original color
        Color currentColor = background.color; // Get the current background color
        float duration = 1f; // Set the desired fade duration
        float elapsedTime = 0f;

        // Fade from current color back to original color over the specified duration
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration; // Calculate the interpolation factor
            background.color = Color.Lerp(currentColor, originalColor, t); // Lerp to the original color
            elapsedTime += Time.deltaTime; // Increase elapsed time

            yield return null; // Wait for the next frame
        }

        // Ensure the final color is set to the original color
        background.color = originalColor;

        // Activate background animation parent after fading
        backgroundAnimParent.gameObject.SetActive(true);
    }

    IEnumerator FadeBackground()
    {
        Debug.Log("startmethod");
        backgroundColor = background.color;
        backgroundAnimParent.gameObject.SetActive(false);


        
        float duration = 1f; // Set your desired fade duration
        float elapsedTime = 0f;

        // Loop until the specified duration has passed
        while (elapsedTime < duration)
        {
            // Calculate the t value for Lerp
            float t = elapsedTime / duration;
            // Lerp to black
            background.color = Color.Lerp(backgroundColor, Color.black, t);
            elapsedTime += Time.deltaTime; // Increase elapsed time

            // Wait for the next frame
            yield return null;
        }

        // Ensure the final color is black
        background.color = Color.black;
        Debug.Log("End method");
    }

}
