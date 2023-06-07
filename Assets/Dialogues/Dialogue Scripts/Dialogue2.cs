using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Dialogue2 : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public TextMeshProUGUI nameTag;
    public List<string> lines;
    public float textSpeed;
    public int index;

    public Button button1;
    public Button button2;
    public Button button3;


    public bool noThird;

    public bool choice;

    public TMP_FontAsset italics;
    public TMP_FontAsset regular;

    public int indexRef;
    public List<int> indexList;
    public List<string> outcome1;
    public List<string> outcome2;
    public List<string> outcome3;
    public int choiceNumber;

    public GameObject blocker;

    public Image NPC;
    public Image arkady;

    [SerializeField] private ButtonScript choiceList;


    // Start is called before the first frame update
    void Start()
    {
        choiceNumber = 0;
        choice = false;
        choiceList.gameObject.SetActive(false);


        //<i>italicized</i>
        //<u> underline </i>


        lines = new List<string>() {"Well, now you know how Numbers work",
        "Take this deck, I have a spare one. Keep it close to your heart",
        "Now if you don't mind, I want to close my eyes for a couple of minutes...",

        "Andreyev. Andreyev, show yourself!",
        "They are taking me away. It is my turn now. Time to go",
        "Accept this as a token of my gratitude for letting me spend some time on this bed",
        "Here. Give it to someone who tried to make the world a better place, but failed. Just like me...",
       
        };



        


        textComponent.text = string.Empty;
        StartDialogue();
    }

    void Awake()
    {

    }



    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index] && !choice)
            {
                NextLine();
            }
            else
            {
                textSpeed = 0.0000000001f;

            }
        }

        if (choice)
        {
            HighlightArkady();
            //Debug.Log("arkady");
        }
        else if (!choice)
        {
            HighlightNPC();
            //Debug.Log("npc");
        }

        
         

        if (index==3) 
        {
            nameTag.text = "Convoy Officer";
            textComponent.font = italics;
        }
        else { 
            
            nameTag.text = "Andreyev";
            textComponent.font = regular;
        
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
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
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



   
}
