using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using System.Collections.Specialized;


public class PrologueText : MonoBehaviour

{
    public TextMeshProUGUI nameBox;
    public TextMeshProUGUI textBox;
    public string[] speakerNames;
    public string[] lines;
    public float textSpeed;
    private int index;
    private bool isTyping;


    void Start()
    {
       textBox.text = string.Empty;
        StartText();
    }

    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            if (isTyping)
            {
               StopAllCoroutines();
               textBox.text = lines[index];
               isTyping = false;
            }
            else
            {
                NextLine();
            }
        
        }
        
    }

    void StartText()
    {
        index = 0;

        ShowLine();
    }

    void ShowLine()

    {
        textBox.text = "";
        nameBox.text = speakerNames[index];
        StartCoroutine(TypeLine());

    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        
        foreach (char c in lines[index])
        {
            textBox.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping=false;
    }

    void NextLine() 
    {
        if (index < lines.Length - 1)
        {
            index++;
            ShowLine();
        
        }
        else gameObject.SetActive(false);
    
    }
}
