using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
            startBattleEnd = false;
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
                StartCoroutine(DelayToNextScene());
            }
        }

    }

    IEnumerator DelayToNextScene()
    {
        yield return new WaitForSeconds(1.5f);
        AkSoundEngine.StopAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartFade()
    {
        //figure out if win or lose
        if(NumberManager.instance.playerVal <= NumberManager.instance.targetVal && NumberManager.instance.oppVal > NumberManager.instance.targetVal && NumberManager.instance.playerVal >= 0)
        {
            image.sprite = victory;
        }
        else if(NumberManager.instance.playerVal <= NumberManager.instance.targetVal && NumberManager.instance.oppVal <= NumberManager.instance.targetVal && Mathf.Abs(NumberManager.instance.targetVal - NumberManager.instance.playerVal) < Mathf.Abs(NumberManager.instance.targetVal - NumberManager.instance.oppVal))
        {
            image.sprite = victory;
        }
        else
        {
            image.sprite = defeat;
            //tie or lose
        }

        elapsed = 0f;
        isFading = true;
    }
}
