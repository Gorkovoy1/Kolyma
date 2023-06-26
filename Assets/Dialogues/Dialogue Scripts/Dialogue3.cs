using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Unity.Burst.Intrinsics;
using UnityEngine.XR;

public class Dialogue3 : MonoBehaviour
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

    public TextMeshProUGUI settingText;


    // Start is called before the first frame update
    void Start()
    {
        settingText.text = "Location";
        choiceNumber = 0;
        choice = false;
        choiceList.gameObject.SetActive(false);


        //<i>italicized</i>
        //<u> underline </i>


        lines = new List<string>() {"Two guards are busy closing the gates behind you. The rest of them are looking at the new prisoners, but their eyes do not express any interest.",
        "It is just another routine procedure in the North. The final batch of convits to close this year.",
        "The prisoners are standing in formation. It is cold, it is hard to stand still.",

        "Prisoners, you have arrived at the North East Labour Camp. This is where you are going to live and work",
        "The Soviet Government is wise and humane. You are convicted criminals and political enemies of the people",
        "Your counterrevolutionary activity and sabotage cannot be forgiven",
        "And yet, the Soviet Government gives you a second chance.",
        "Do not forget what Comrade Stalin once said: Labor in the USSR is a matter of honor, glory, courage and heroism!",
        "Work for the sake of the Soviet state, show your enthusiasm, join the proletariate and do your time with honor",
        "Only through hardships and labor you will find your way to join the Sovier society and step together into the bright Socialist Future",
            "Hah, look at them. Fresh meat!",
         "Alright, we are done here. Get your uniform, go to your assigned barracks and await further orders! Now move!"
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




        if (index == 3)
        {
            nameTag.text = "Convoy Officer";
            textComponent.font = italics;
        }
        else
        {

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
        if(index == 0)
        {
            yield return new WaitForSeconds(4.3f);
        }
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
