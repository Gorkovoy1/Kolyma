using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public bool choice;
    public TextMeshProUGUI textComponent;
    public List<string> lines;
    public List<int> indexList;
    public List<string> outcome1;
    public List<string> outcome2;
    public List<string> outcome3;
    public int choiceNumber;
    public float textSpeed;
    public int index;
    public Button button1;
    public Button button2;
    public Button button3;
    public int indexRex;
    [SerializeField] private ChoiceButton choiceButton;
    // Start is called before the first frame update
    void Start()
    {
        choice = false;
        choiceNumber = 0;
        lines = new List<string>() { "Don't worry, I know the rules. You were here first and the bed is yours. But can I still sit here? It has been a long and tiring journey.",
        "",
        "It's just unthikable! A week on a train, two days on a boat and here we are.",
            "I have been to this part of Russia before, but never could I imagine I'd have to travel all the way to Magadan again.",
            "My last name is Andreev, my first name is Aleskander.",
            "",
            "It is quite ironic. I was arrested for anti-government activity in 1905 and I am an enemy of the Soviet state today. Things did not change for me that much",
            "Our Revolution was a dream that never came true. Well, you are here, there's no need to tell you that. What is your name?"

        };
        outcome1 = new List<string>()
        {
            "Oh, I am so grateful.", "Yes I was. A member since 1901"
        };

        outcome2 = new List<string>()
        {
            "Rules are rules. Although I really doubt anyone will be able to catch some sleep in such a packed cell.",
            "I did. But they lured me into a trap. I have no regrets though, I'd rather die here than in France, like one of those impotent immigrants"
        };
        outcome3 = new List<string>()
        { "Your heart is soft. Don't worry, it will not stay that way for too long",
        "It sure was. But still, you should be careful who you trust."
        };
        indexList = new List<int>() { 1, 5, 8 };

        textComponent.text = string.Empty;
        StartDialogue();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index] && !choice)
            {
                NextLine();
            }
            else {
                textSpeed = 0.00001f;
           
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
        choice = false;
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        if (textComponent.text == lines[index] && index == 0) 
        {
            DisplayChoices("Sure. You may sit down", "You may sit down. But do not forget you're sleeping on the floor tonight", "Of course. Sit down here, it's hard to stand in such a crowded room");
            yield return null;
        }
        if (textComponent.text == lines[index] && index == 4)
        {
            DisplayChoices("I know you. You were a member of the Socialist Revolutionary Party", "Former terrorist? I thought you left the country", "I heard you were a spy. I am sure it was a lie.");
            yield return null;
        }
    }
    void NextLine()
    {
        if (index < lines.Count - 1)
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
        choiceButton.gameObject.SetActive(true);
        choiceButton.button1.onClick.AddListener(FirstClicked);
        choiceButton.button2.onClick.AddListener(SecondClicked);
        choiceButton.button3.onClick.AddListener(ThirdClicked);
        choiceButton.GetComponent<ChoiceButton>().option1 = first;
        choiceButton.GetComponent<ChoiceButton>().option2 = second;
        choiceButton.GetComponent<ChoiceButton>().option3 = third;
    }
    public void FirstClicked()
    {
        choice = false;
        choiceButton.gameObject.SetActive(false);
        lines[indexList[choiceNumber]] = outcome1[choiceNumber];
      choiceNumber++;
      NextLine();
    }
    public void SecondClicked()
    {
        choice = false;
        choiceButton.gameObject.SetActive(false);
        lines[indexList[choiceNumber]] = outcome2[choiceNumber];
        choiceNumber++;
        NextLine();
    }
    public void ThirdClicked()
    {
        choice = false;
        choiceButton.gameObject.SetActive(false);
        lines[indexList[choiceNumber]] = outcome3[choiceNumber];
        choiceNumber++;
        NextLine();
    }
}
