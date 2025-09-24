using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleEndController : MonoBehaviour
{
    public Sprite victory;
    public Sprite defeat;

    public bool startBattleEnd = false;

    public Image image;
    public float fadeTime;
    private float elapsed = 0f;
    public bool isFading = false;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startBattleEnd)
        {
            StartFade();
        }

        if (isFading)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeTime);
            Color color = image.color;
            color.a = alpha;
            image.color = color;

            if (alpha >= 1f)
            {
                isFading = false; // Fade finished
            }
        }

    }

    public void StartFade()
    {
        startBattleEnd = false;
        elapsed = 0f;
        isFading = true;
    }
}
