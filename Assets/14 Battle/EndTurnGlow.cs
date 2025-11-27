using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnGlow : MonoBehaviour
{
    public Material mat;
    public string outlinePropertyName = "_ImageOutline";
    public bool isFlashing = false;
    public float elapsedTime = 0f;
    public float fadeDuration = 1f;
    public float timeOne;
    public float timeTwo;
    public bool timerStart;


    void Start()
    {
        SetAlpha(0f);
    }

    void Update()
    {
        if(TurnManager.instance.playerPlayedCard && !isFlashing && !timerStart)
        {
            timerStart = true;
            timeOne = 0f;
            
            
        }

        if(timerStart)
        {
            if (timeOne < timeTwo)
            {
                timeOne += Time.deltaTime;
            }
            else
            {
                isFlashing = true;
                timerStart = false;
            }
        }

        if(!isFlashing)
        {
            SetAlpha(0f);
            return;
        }


        // Increases over time
        elapsedTime += Time.deltaTime;

        // PingPong goes 0→1→0 smoothly
        float t = Mathf.PingPong(elapsedTime / fadeDuration, 1f);

        SetAlpha(t);
    }

    public void SetAlpha(float alpha)
    {
        Color c = mat.GetColor(outlinePropertyName);

        c.a = alpha;

        mat.SetColor(outlinePropertyName, c);
    }

    public void ToggleFlash()
    {
        isFlashing = !isFlashing;
    }

    
}
