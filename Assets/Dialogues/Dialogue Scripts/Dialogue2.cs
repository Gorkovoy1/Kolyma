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
    public Image panel;
    public Image nameTagPanel;

    public Color panelColor;

    public bool isItalic;
    public bool endOfString;
    public bool isRed;

    public TMP_FontAsset numbersFont;

    [SerializeField] private ButtonScript choiceList;


    // Start is called before the first frame update
    void Start()
    {
        choiceNumber = 0;
        choice = false;
        choiceList.gameObject.SetActive(false);
        panelColor = panel.GetComponent<Image>().color;

        //<i>italicized</i>
        //<u> underline </i>


        lines = new List<string>() {"Well, now you know how Numbers work.",
        "Take this deck, I have a spare one. Keep it close to your heart.",
        "Now if you don't mind, I want to close my eyes for a couple of minutes...",
        "",
        "",
        "Andreyev. Andreyev, show yourself!",
        "They are taking me away. It is my turn now. Time to go.",
        "Accept this as a token of my gratitude for letting me spend some time on this bed.",
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
            if (endOfString && !choice && index != 3)
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

        
         

        if (index==5) 
        {
            nameTag.text = "Convoy Officer";
            textComponent.font = italics;
            NPC.gameObject.SetActive(false);
            arkady.gameObject.SetActive(false);
            Color opaquePanel = new Color(panelColor.r, panelColor.g, panelColor.b, 1f);
            panel.GetComponent<Image>().color = opaquePanel;
            nameTagPanel.GetComponent<Image>().color = opaquePanel;
        }
        else if (index != 3) 
        { 
            NPC.gameObject.SetActive(true);
            arkady.gameObject.SetActive(true);
            nameTag.text = "Andreyev";
            textComponent.font = regular;
        }


        
    }  
    
    IEnumerator FadeToBlack(Image panel)
    {
        float fadeDuration = 1f; // Duration of the fade effect in seconds
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            panelColor.a = alpha; // Update the alpha value of the original panelColor
            panel.GetComponent<Image>().color = panelColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1.8f);

        panelColor.a = 1f; // Set alpha value back to 1 (fully opaque)
        panel.GetComponent<Image>().color = panelColor;
        NextLine();
        // Once the fade is complete, you can perform any desired actions
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
        
        if (index == 3)
        {
            NPC.gameObject.SetActive(false);
            arkady.gameObject.SetActive(false);
            StartCoroutine(FadeToBlack(panel));
            StartCoroutine(FadeToBlack(nameTagPanel));
        }
        else
        {
            Color opaquePanel = new Color(panelColor.r, panelColor.g, panelColor.b, 1f);
            panel.GetComponent<Image>().color = opaquePanel;
            nameTagPanel.GetComponent<Image>().color = opaquePanel;
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
