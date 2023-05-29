using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public List<string> lines;
    public float textSpeed;
    public int index;

    public Button button1;
    public Button button2;
    public Button button3;

    
    public bool noThird;

    public bool choice;


    public int indexRef;
    public List<int> indexList;
    public List<string> outcome1;
    public List<string> outcome2;
    public List<string> outcome3;
    public int choiceNumber;

    public GameObject blocker;

    [SerializeField] private ButtonScript choiceList;


    // Start is called before the first frame update
    void Start()
    {
        choiceNumber = 0;
        choice = false;
      


    //<i>italicized</i>



    lines = new List<string>() {"Don't worry, I know the rules. You were here first and the bed is yours. But can I still sit here? It has been a long and tiring journey.",
        "",
        "It's just unthinkable! A week on a train, two days on a boat and here we are.",
        "I have been to this part of Russia before, but never could I imagine I'd have to travel all the way to Magadan again.",
        "My last name is Andreev, my first name is Aleksander.",
        "",
        "It is quite ironic. I was arrested for anti-government activity in 1905 and I am an enemy of the Soviet state today. Things did not change for me that much.",
        "Our Revolution was a dream that never came true. Well, you are here, there's no need to tell you that. What is your name?",
        "",
        "",
        "I am sorry. Let me give you some advice. Put your former self to slumber. Focus on what is important now.",
        "Try to kill the time and not to kill yourself in the process. You can trust me on this one, I know what I am talking about.",
        "Let's play a simple game. It is very popular around here so you better remember the rules. It might even save your life one day so listen carefully."
        };



        indexList = new List<int>() {1, 5, 8, 9};

        outcome1 = new List<string>() {"Oh, I am so grateful.", 
        "Yes I was. A member since 1901.", "If you don't mind me asking, was General Kojukh your father?", "His trial was a joke, and you could tell how badly those miserable party clerks wanted to get rid of him."};


        outcome2 = new List<string>() {"Rules are rules. Although I really doubt anyone will be able to catch some sleep in such a packed cell.", 
        "I did. But they lured me into a trap. I have no regrets though, I'd rather die here than in France, like one of those impotent immigrants.", "As you wish. As far as I have heard, someone here called you Arkady. That's good enough for me.", ""};
        
        
        outcome3 = new List<string>() {"Your heart is soft. Don't worry, it will not stay that way for too long.",
        "It sure was. But still, you should be careful who you trust.", "", ""};


        textComponent.text = string.Empty;
        StartDialogue();
    }

    void Awake()
    {
        
    }

    

    void Update()
    {
        
            if(Input.GetMouseButtonDown(0))
            {
                if(textComponent.text == lines[index] && !choice)
                {
                    NextLine();
                }
                else 
                {
                    textSpeed = 0.0000000001f;
                    
                }
            }    
        
            
        
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
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        
        if(textComponent.text == lines[index] && index == 0)
        {
            DisplayChoices("Sure. You may sit down.", "You may sit down. But do not forget you're sleeping on the floor tonight.", "Of course. Sit down here, it's hard to stand in such a crowded room.");
            yield return null;
        }

        if(textComponent.text == lines[index] && index == 4)
        {
            DisplayChoices("I know you. You were a member of the Socialist Revolutionary Party.", "Former terrorist? I thought you left the country.", "I heard you were a spy. I am sure it was a lie.");
            yield return null;
        }

        if(textComponent.text == lines[index] && index == 7)
        {
            DisplayChoices("Arkady Kojukh.", "No need to know my name.", "");
            button3.gameObject.SetActive(false);
            
        }

        if(lines[index] == "")
        {
            NextLine();
        }

    }

    void NextLine()
    {
        if(index < lines.Count - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    

    public void DisplayChoices(string first, string second, string third)
    {
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

        choiceList.GetComponent<ButtonScript>().choice1 = first;
        choiceList.GetComponent<ButtonScript>().choice2 = second;
        choiceList.GetComponent<ButtonScript>().choice3 = third;

    }

    private void firstClicked()
    {
        choice = false;    
        Debug.Log("first");
        choiceList.gameObject.SetActive(false);
        lines[indexList[choiceNumber]] = outcome1[choiceNumber];
        choiceNumber++;
        button3.gameObject.SetActive(true);

        if(lines[8] == outcome1[2] && choiceNumber == 3)
        {
            NextLine();
            DisplayChoices("General Valentin Kojukh was my father.", "No. I do not have a father.", "He was. He was arrested for no reason.");
        }
        else
        {
            NextLine();
        }
        
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
