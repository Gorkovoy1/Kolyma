using UnityEngine;
using System.Collections;

public class UIScreenShake : MonoBehaviour
{
    RectTransform rect;
    Vector2 originalPos;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalPos = rect.anchoredPosition;
    }

    public void Shake(float duration, float strength)
    {
        StartCoroutine(ShakeCoroutine(duration, strength));
    }

    IEnumerator ShakeCoroutine(float duration, float strength)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            rect.anchoredPosition =
                originalPos +
                Random.insideUnitCircle * strength;

            yield return null;
        }

        rect.anchoredPosition = originalPos;
    }
}