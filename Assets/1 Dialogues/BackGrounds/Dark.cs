using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dark : MonoBehaviour
{
    public Image dark;
    public Color darkColor;
    public float duration;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        darkColor = dark.color;
        dark.color = new Color(darkColor.r, darkColor.g, darkColor.b, 1f);
        StartCoroutine(DarkScreen());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DarkScreen()
    {
        yield return new WaitForSeconds(duration);

        while(dark.color.a > 0f)
        {
            dark.color = new Color(darkColor.r, darkColor.g, darkColor.b, dark.color.a - speed * Time.deltaTime);
            yield return null;
        }

        dark.color = new Color(darkColor.r, darkColor.g, darkColor.b, 0f);

        Destroy(dark.gameObject);
    }

}
