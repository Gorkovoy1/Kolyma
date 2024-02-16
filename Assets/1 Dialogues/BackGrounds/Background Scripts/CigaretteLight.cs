using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CigaretteLight : MonoBehaviour
{
    public Image cigaretteLight;
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
        cigaretteLight.GetComponent<CanvasGroup>().alpha = randomalpha;
        Debug.Log("change");
        yield return new WaitForSeconds(AnimationTime);
        StartCoroutine(ChangeOpacity());    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
