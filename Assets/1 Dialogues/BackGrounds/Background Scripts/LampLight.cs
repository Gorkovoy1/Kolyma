using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LampLight : MonoBehaviour
{
    public Image lampLight;
    public float MinimumTime;
    public float MaximumTime;
    public float MinAlpha;
    public float MaxAlpha;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ChangeLampOpacity());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ChangeLampOpacity()
    {
        float randominterval = Random.Range(MinimumTime, MaximumTime);
        lampLight.GetComponent<CanvasGroup>().alpha = MinAlpha;
        yield return new WaitForSeconds(randominterval);
        randominterval = Random.Range(MinimumTime, MaximumTime);
        lampLight.GetComponent<CanvasGroup>().alpha = MaxAlpha;
        yield return new WaitForSeconds(randominterval);
        StartCoroutine(ChangeLampOpacity());    
    }
}
