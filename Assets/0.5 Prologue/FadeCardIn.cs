using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class FadeCardIn : MonoBehaviour
{
    private Image image;
    [SerializeField] float fadeDuration = 0.5f;
    RectTransform rect;
    Vector2 originalPos;
    [SerializeField] float startOffset = 1000f;
    [SerializeField] float duration = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        rect = GetComponent<RectTransform>();
        originalPos = rect.anchoredPosition;

        image = GetComponent<Image>();

        Color c = image.color;
        c.a = 0f;
        image.color = c;

        rect.anchoredPosition = originalPos + Vector2.right * startOffset;

        StartCoroutine(SlideIn());
        StartCoroutine(FadeIn());

    }

    IEnumerator SlideIn()
    {
        Vector2 startPos = rect.anchoredPosition;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            rect.anchoredPosition =
                Vector2.Lerp(startPos, originalPos, timer / duration);

            yield return null;
        }

        rect.anchoredPosition = originalPos;
    }

    IEnumerator FadeIn()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            Color c = image.color;
            c.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            image.color = c;

            yield return null;
        }

        Color finalColor = image.color;
        finalColor.a = 1f;
        image.color = finalColor;
    }
}
