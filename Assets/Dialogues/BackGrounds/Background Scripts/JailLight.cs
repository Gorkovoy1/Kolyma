using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JailLight : MonoBehaviour
{
    public Image TopLight;
    public float AnimationTime;
    public float MinimumAlpha;
    public float MaximumAlpha;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ChangeOpacity());
           
        
    }

    IEnumerator ChangeOpacity()
    
    {
        float randomalpha = Random.Range(MinimumAlpha, MaximumAlpha);
        TopLight.GetComponent<CanvasGroup>().alpha = randomalpha;
        yield return new WaitForSeconds(AnimationTime);
        StartCoroutine(ChangeOpacity());    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
