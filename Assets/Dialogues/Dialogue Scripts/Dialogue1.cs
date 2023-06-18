using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Dialogue1 : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public TextMeshProUGUI nameTag;
    public GameObject nameTagBox;
    public List<string> lines;
    public float textSpeed;
    public int index;

    public Button button1;
    public Button button2;
    public Button button3;

    public Animator animator;

    
    public bool noThird;

    public bool choice;

    public bool endOfString;
    
    public bool isItalic;
    public bool isRed;


    public int indexRef;
    public List<int> indexList;
    public List<string> outcome1;
    public List<string> outcome2;
    public List<string> outcome3;
    public List<string> introText;
    public int choiceNumber;

    public GameObject blocker;

    public Image NPC;
    public Image arkady; 

    public GameObject levelLoader;

    public TMP_FontAsset numbersFont;

    [SerializeField] private ButtonScript choiceList;


    // Start is called before the first frame update
    void Start()
    {
        choiceNumber = 0;
        choice = false;
        choiceList.gameObject.SetActive(false);


    //<i>italicized</i>


    introText = new List<string>()
        {
            "There were too many people in this jail. The prisoners took all the bunks, occupied all the space under the beds and around them.",
            "The air was stuffy. The smell of wet clothes emanated from the cells.",
            "Everyone was tired. Those who could not lie down, tried to lean on something.",
            "Arkady was shivering on his bottom bunk. He was about to turn on the side and try to fall asleep when suddenly he felt that someone was looking at him.",
            "An old man approached his bed. The stranger had a strong presence. He was exhasted like everyone and yet Arkady could tell that his spirit was not broken."
        };

    lines = new List<string>() {"Don't worry, I know the rules. You were here first and the bed is yours. But can I still sit here? I never ask for anything, but it's been a long and tiring journey.",
        "",
        "Just think about it! 45 days on a train, 2 days on a boat and here we are.",
        "I've been to this part of Russia before, but never could I imagine I'd have to travel all the way to Magadan again.",
        "I am Andreyev, Alexander Andreyev.",
        "",
        "It is quite ironic. The Imperial Okhrana came after me in 1907. And I am an enemy of the Soviet state today, in 1938.",
        "Things did not change for me that much!",
        "All those sentimental idiots say that our Revolution was a dream that never came true!",
        "The Revolution was first and foremost an opportunity. We had our chance and we missed it... What is your name?",
        "",
        "",
        "I am sorry... But let me give you some advice. You are a prisoner now, so forget about your former self. Focus on more important matters.",
        "Let's play a simple game. It will keep our minds busy and help us to kill the time.",
        "The game is called Numbers. It is very popular among the prisoners so you better remember the rules. It might even save your life one day so listen carefully."
        };



        indexList = new List<int>() {1, 5, 10, 11};

        outcome1 = new List<string>() {"Oh, I am so grateful!", 
        "Yes I was. A member since 1901.", "If you don't mind me asking, was General Kojukh your father?", "His trial was a joke, and you could tell how badly those miserable party clerks wanted to get rid of him."};


        outcome2 = new List<string>() {"Rules are rules. Although I really doubt anyone will be able to catch some sleep in such a packed cell.", 
        "I did. But they lured me into a trap. I have no regrets though, I'd rather die here than in France, like one of those impotent pathetic immigrants.", "As you wish. As far as I have heard, someone here called you Arkady. That's good enough for me.", ""};
        
        
        outcome3 = new List<string>() {"Hm. I thank you. Your heart is soft. Don't worry, it won't stay that way for too long.",
        "It sure was. They are calling everyone a spy now. They need us, the scapegoats, for the Purge to happen. Ha! One day they will pay for this!", "", ""};



        GameObject levelLoader = GameObject.Find("LevelLoader");
        arkady.gameObject.SetActive(false);
        NPC.gameObject.SetActive(false);
        nameTagBox.gameObject.SetActive(false);

        StartCoroutine(Intro());

        

    }

    IEnumerator Intro()
    {
        yield return new WaitForSeconds(4.3f);
        textComponent.text = string.Empty;
        for(int i = 0; i < 5; i++)
        {
            
            foreach(char c in introText[i].ToCharArray())
            {
                textComponent.text += c;
                yield return new WaitForSeconds(0.03f);
            }
            bool isWaitingForClick = true;
            while(isWaitingForClick)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    isWaitingForClick = false;
                }
            
                    yield return null;
            
            }
            textComponent.text = string.Empty;
        }
        textComponent.text = string.Empty;
        
        StartDialogue();
        arkady.gameObject.SetActive(true);
        NPC.gameObject.SetActive(true);
        nameTagBox.gameObject.SetActive(true);
    }

  

    void Awake()
    {
        
    }

    

    void Update()
    {
        
            if(Input.GetMouseButtonDown(0))
            {
                if(endOfString && !choice)
                {
                    NextLine();
                }
                else 
                {
                    textSpeed = 0.0000000001f;
                    
                }
            }    
        
            if(choice)
            {
                HighlightArkady();
                //Debug.Log("arkady");
            }
            else if(!choice)
            {
                HighlightNPC();
                //Debug.Log("npc");
            }

            if (index < 4)
            {
                nameTag.text = "Old Man";

            }
            else 
            {
                nameTag.text = "Andreyev"; 
        
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

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());

    }

    IEnumerator TypeLine()
    {
        textSpeed = 0.03f;
        choice = false;
        endOfString = false;
        char[] charArray = lines[index].ToCharArray();
        for(int i = 0; i < charArray.Length; i++)
        {
            char c = charArray[i];
            if(c == ' ')
            {
                isItalic = false;
                isRed = false;
            }
            if (c == 'N')
            {
                isRed = true;            
            }
            if(c == '<')
            {
                isItalic = true;
                continue;
            }

            if(isItalic)
            {
                textComponent.text += "<i>" + c + "</i>";
            }
            else if(isRed)
            {
                //textComponent.text += "<color=blue>" + c + "</color>";
                textComponent.text += "<font=\"" + numbersFont.name + "\">" + c + "</font>";
                
            }
            else
            {
                textComponent.text += c;
            }

            yield return new WaitForSeconds(textSpeed);

            if (i == charArray.Length - 1)
            {
                // String is fully displayed, perform additional actions
                endOfString = true;
            }
        }
        
        if(endOfString && index == 0)
        {
            //levelLoader.gameObject.SetActive(false);
            DisplayChoices("Sure. You may sit down.", "You may sit down. But do not forget you're sleeping on the floor tonight.", "Of course. Sit down here, it's hard to stand in such a crowded room.");
            yield return null;
        }

        if(endOfString && index == 4)
        {
            DisplayChoices("I know you. You were a member of the Socialist Revolutionary Party.", "Former terrorist? I thought you left the country.", "I heard you were a spy. I am sure it was a lie.");
            yield return null;
        }

        if(endOfString && index == 9)
        {
            DisplayChoices("Arkady Kojukh.", "No need to know my name.", "");
            button3.gameObject.SetActive(false);
            
        }

        if(lines[index] == "")
        {
            NextLine();
        }

        
        if(textComponent.text == "If you don't mind me asking, was General Kojukh your father?")
        {
            DisplayChoices("General Valentin Kojukh was my father.", "No. I do not have a father.", "He was. He was arrested for no reason.");
        }
        

    }

    void NextLine()
    {
        endOfString = false;
        if(index < lines.Count - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            //levelLoader.gameObject.SetActive(true);
            animator.SetBool("SceneEnd", true);
            animator.SetBool("SceneEnd", false);
            animator.Play("Crossfade_Start");
            //gameObject.SetActive(false);
            //NPC.gameObject.SetActive(false);
            //arkady.gameObject.SetActive(false);
            StartCoroutine(NextScene());
        }
    }

    IEnumerator NextScene()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Test");
    }

    

    public void DisplayChoices(string first, string second, string third)
    {
        choiceList.GetComponent<ButtonScript>().choice1 = first;
        choiceList.GetComponent<ButtonScript>().choice2 = second;
        choiceList.GetComponent<ButtonScript>().choice3 = third;

        choice = true;
        choiceList.gameObject.SetActive(true);
        if(noThird)
        {
            choiceList.thirdChoice.gameObject.SetActive(false);
        }
        button1.onClick.RemoveAllListeners();
        button2.onClick.RemoveAllListeners();
        button3.onClick.RemoveAllListeners();
        button1.onClick.AddListener(firstClicked);
        button2.onClick.AddListener(secondClicked);
        button3.onClick.AddListener(thirdClicked);

        
    }

    private void firstClicked()
    {
        choice = false;    
        Debug.Log("first");
        choiceList.gameObject.SetActive(false);
        lines[indexList[choiceNumber]] = outcome1[choiceNumber];
        choiceNumber++;
        button3.gameObject.SetActive(true);
        
        NextLine();
        
        
        
    }

    private void secondClicked()
    {
        choice = false;
        Debug.Log("second");
        choiceList.gameObject.SetActive(false);
        lines[indexList[choiceNumber]] = outcome2[choiceNumber];
        choiceNumber++;
        button3.gameObject.SetActive(true);
        NextLine();
    }

    private void thirdClicked()
    {
        choice = false;
        Debug.Log("third");
        choiceList.gameObject.SetActive(false);
        lines[indexList[choiceNumber]] = outcome3[choiceNumber];
        choiceNumber++;
        button3.gameObject.SetActive(true);
        NextLine();
    }
}
