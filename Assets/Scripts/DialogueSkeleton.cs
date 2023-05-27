using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DialogueSkeleton : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public List<string> lines;
    public float textSpeed;
    public int index;

    public Button button1;
    public Button button2;
    public Button button3;

    public bool choice;


    public int indexRef;
    public List<int> indexList;
    public List<string> outcome1;
    public List<string> outcome2;
    public List<string> outcome3;
    public int choiceNumber;

    [SerializeField] private ButtonScript choiceList;


    void Start()
    {
        choiceNumber = 0;
        choice = false;
        
        //<i>italicized</i>

        lines = new List<string>() {

        //fill this list with each line the NPC will say
        //whenever there is a line that has different variations depending on player choice, leave it blank (make it an empty string)
        //put each dialogue line in the list in a different line for easy organization
	


        };



        indexList = new List<int>() {          
        
        //fill this list with the index positions of the empty strings in the lines list
        //whichever position is blank, put that position here

        };

        outcome1 = new List<string>() {
        
        //fill this list with all the NPC responses if the player picks the first button/choice, in order
        
        };


        outcome2 = new List<string>() {
        
        //fill this list with all the NPC responses if the player picks the second button/choice, in order
        
        };
        
        
        outcome3 = new List<string>() {
        
        //fill this list with all the NPC responses if the player picks the third button/choice, in order

        };


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
        
        if(textComponent.text == lines[index] && index == 0) /* put first position of empty string in lines, minus 1 */ // 
        {
            DisplayChoices("x", "y", "z"); /* in here, put the three choices that the player will have, and separate these strings with commas - if no third choice use empty string and set button 3 to inactive */ 
            yield return null;
        }

        if(textComponent.text == lines[index] && index == 7) /* next position of empty string in lines minus 1 */ 
        {
            DisplayChoices("x", "y", "z");
            yield return null;
        }

        //keep duplicating the above block for any consequent choices

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
        choiceList.gameObject.SetActive(false);
        lines[indexList[choiceNumber]] = outcome1[choiceNumber];
        choiceNumber++;
        button3.gameObject.SetActive(true);
        NextLine();

        //will need if else statement if have place where there is an extra choice if button 1 is picked
        
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

