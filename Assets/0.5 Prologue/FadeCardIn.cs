using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class FadeCardIn : MonoBehaviour
{
    private Image image;
    [SerializeField] float fadeDuration = 0.5f;
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
        image = GetComponent<Image>();

        Color c = image.color;
        c.a = 0f;
        image.color = c;

        StartCoroutine(FadeIn());
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
