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
    public float fadeDuration = 1.2f;

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
                Color color = textBox.color;
                color.a = 1f;
                textBox.color = color;
                StartCoroutine(FinishTypingNextFrame());
            }
        }
        
    }

    IEnumerator FinishTypingNextFrame()
    {
        yield return new WaitForSeconds(0.1f);
        isTyping = false;
    }

    public void ShowLine()

    {
        textBox.text = "";
        nameBox.text = name;
        isTyping = true;
        typingRoutine = StartCoroutine(DisplayLine());

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

    IEnumerator DisplayLine()
    {
        textBox.text = text;
        Color color = textBox.color;
        color.a = 0f;
        textBox.color = color;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            textBox.color = color;

            yield return null;
        }

        color.a = 1f;
        textBox.color = color;
        isTyping = false;
    }
}
