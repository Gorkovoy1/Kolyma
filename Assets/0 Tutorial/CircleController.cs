using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleController : MonoBehaviour
{
    public Image circle;
    public float fillDuration = 2f;

    // Start is called before the first frame update
    void Start()
    {
        circle = GetComponent<Image>();
        StartCoroutine(CircleAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    IEnumerator CircleAnimation()
    {
        float elapsed = 0f;
        circle.fillAmount = 0f; // start empty

        while (elapsed < fillDuration)
        {
            elapsed += Time.deltaTime;
            circle.fillAmount = Mathf.Clamp01(elapsed / fillDuration);
            yield return null; 
        }

        circle.fillAmount = 1f; 
    }
}

