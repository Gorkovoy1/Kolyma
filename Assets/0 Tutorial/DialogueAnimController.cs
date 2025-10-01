using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueAnimController : MonoBehaviour
{
    public Image portraitImage;
    public Image textBoxImage;
    public bool portraitDone;
    public bool textBoxDone;
    public float fillSpeed1;
    public float fillSpeed2;
    public bool textBoxStarted;
    public TMP_Text dialogueText;
    public float elapsedTime;
    public float fadeDuration;
    public RectTransform dialogueRect;
    public bool moved = false;

    // Start is called before the first frame update
    void OnEnable()
    {
        portraitImage.fillAmount = 0f;
        textBoxImage.fillAmount = 0f;
        moved = false;
        portraitDone = false;
        textBoxDone = false;
        textBoxStarted = false;
        Color c = dialogueText.color;
        c.a = 0f;
        dialogueText.color = c;
        elapsedTime = 0f;
        dialogueRect = dialogueText.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {

        if(!portraitDone)
        {
            portraitImage.fillAmount += fillSpeed1 * Time.deltaTime;

            if(portraitImage.fillAmount > 0.43f)
            {
                textBoxStarted = true;
            }

            if (portraitImage.fillAmount >= 1f)
            {
                portraitImage.fillAmount = 1f;
                portraitDone = true;
            }
            

        }
        if (portraitImage.fillAmount > 0.43f)
        {
            textBoxStarted = true;
        }
        if (textBoxStarted && !textBoxDone)
        {
            textBoxImage.fillAmount += fillSpeed2 * Time.deltaTime;

            if(textBoxImage.fillAmount >= 1f)
            {
                textBoxImage.fillAmount = 1f;
                textBoxDone = true;
            }
        }

        if (elapsedTime < fadeDuration && textBoxDone)
        {
            elapsedTime += Time.deltaTime;

            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            Color c = dialogueText.color;
            c.a = alpha;
            dialogueText.color = c;
        }

        if(!portraitImage.enabled && !moved)
        {
            moved = true;
            dialogueRect.anchoredPosition = new Vector2(-40f, 0f);
        }
        else if(portraitImage.enabled)
        {
            dialogueRect.anchoredPosition = new Vector2(-3f, -3f);
        }
        
    }
}
