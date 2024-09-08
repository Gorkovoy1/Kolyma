using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnaceLight : MonoBehaviour
{
    public Image furnaceLight;
    public float MinimumTime;
    public float MaximumTime;
    public float MinAlpha;
    public float MaxAlpha;

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(ChangeFurnaceOpacity());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ChangeFurnaceOpacity()
    {
        float randominterval = Random.Range(MinimumTime, MaximumTime);
        furnaceLight.GetComponent<CanvasGroup>().alpha = MinAlpha;
        yield return new WaitForSeconds(randominterval);
        randominterval = Random.Range(MinimumTime, MaximumTime);
        furnaceLight.GetComponent<CanvasGroup>().alpha = MaxAlpha;
        yield return new WaitForSeconds(randominterval);

        StartCoroutine(ChangeFurnaceOpacity());    
    }

}


    