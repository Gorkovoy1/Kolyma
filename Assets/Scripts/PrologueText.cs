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
    private Coroutine typingRoutine;

    void Start()
    {
        
    }

    
    void Update()
    {
        if(isTyping)
        {
            if(Input.GetMouseButtonDown(0))
            {
                StopCoroutine(typingRoutine);

                textBox.text = text;
                StartCoroutine(FinishTypingNextFrame());
            }
        }
        
    }

    IEnumerator FinishTypingNextFrame()
    {
        yield return null;
        isTyping = false;
    }

    public void ShowLine()

    {
        textBox.text = "";
        nameBox.text = name;
        typingRoutine = StartCoroutine(TypeLine());

    }

    IEnumerator TypeLine()
    {
        textSpeed = 0.042f;
        isTyping = true;
        
        foreach (char c in text)
        {
            textBox.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    
}
