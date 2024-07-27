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
    public string musicName;
    public SpriteRenderer NPCBlack;
    public SpriteRenderer NPCColor;
    public TextMeshProUGUI loaderText;
    public GameObject backgroundAnimParent;
    public GameObject screenSFXParent;
    public GameObject ambientParent;

    public DialogueScriptableObject dialogueObj1;
    public DialogueScriptableObject dialogueObj2;

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
            inkStory = new Story(inkJSON.text);
            string savedState = PlayerPrefs.GetString("SavedInkState");
            inkStory.state.LoadJson(savedState);
            GameObject levelLoader = GameObject.Find("LevelLoader");
            levelLoader.gameObject.SetActive(false);
            introText.text = "";
            PlayerPortrait.gameObject.SetActive(false);
            NPCPortrait.gameObject.SetActive(false);
            StartCoroutine(ShowInkStory());
            //not sure if true or false
            startofDialogue = true;
            soundEnded = true;
            playSound = false;
            ambientObj.SetActive(true);
            AkSoundEngine.PostEvent(musicName, gameObject);
            skip = false;
            paperColor = paper.color;
            paper.color = new Color(paperColor.r, paperColor.g, paperColor.b, 1f);
            narratorTag.gameObject.SetActive(false);
            //??? change?
            smoke = backgroundAnimParent;
            smoke.gameObject.SetActive(true);

        }

        else
        {
            //create ink file and set level loader. deactivate everything while levelloader is active, reset bools and set paper to invisible
            inkStory = new Story(inkJSON.text);
            GameObject levelLoader = GameObject.Find("LevelLoader");
            levelLoader.gameObject.SetActive(true);
            introText.text = "";
            PlayerPortrait.gameObject.SetActive(false);
            NPCPortrait.gameObject.SetActive(false);
            startScene = true;
            StartCoroutine(ShowInkStory());
            startofDialogue = true;
            soundEnded = true;
            playSound = false;
            ambientObj.SetActive(false);
            Debug.Log("Ambient Off");
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
        


    }

    void DialogueSetUp()
    {
        
        if(dialogueNumber == 1)
        {
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
            //set loader text
            loaderText.text = dialogueObj1.line1 + Environment.NewLine + Environment.NewLine + dialogueObj1.line2;
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
            //set loader text
            loaderText.text = dialogueObj2.line1 + Environment.NewLine + Environment.NewLine + dialogueObj2.line2;
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
                if (tags[0] == "Andreyev" || tags[0] == "Rybakov")
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

                    //this executes before the line finishes
                    //PauseScene();
                }

                if (tags[0] == "ReceiveItem")
                {
                    NPCPortrait.gameObject.SetActive(false);
                    AkSoundEngine.PostEvent("Play_Woosh_Narrator",gameObject);
                    PlayerPortrait.gameObject.SetActive(false);
                    narratorTag.gameObject.SetActive(true);

                    //this executes before the line finishes
                    //LoadNextScene();
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
                //this is a test
                if(tags[0] == "Tutorial"){
                        //pause this scene and temporarily swtich to other scene
                        PauseScene();
                }
                else if(tags[0] == "ReceiveItem"){
                        LoadNextScene();
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

        SceneManager.LoadScene("TestScene");
    }

    //end of dialogue
    void LoadNextScene()
    {
        PlayerPrefs.DeleteKey("SavedInkState");
        SceneManager.LoadScene(nextSceneNumber);
    }

}
