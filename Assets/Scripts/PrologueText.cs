using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using System.Collections.Specialized;


public class PrologueText : MonoBehaviour

{
    public string name;
    public string text;
    public float textSpeed;
    public bool isTyping;
    public TextMeshProUGUI textBox;
    public TextMeshProUGUI nameBox;


    void Start()
    {
        
    }

    
    void Update()
    {
        
        
    }

    

    public void ShowLine()

    {
        textBox.text = "";
        nameBox.text = name;
        StartCoroutine(TypeLine());

    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        
        foreach (char c in text)
        {
            textBox.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    
}
