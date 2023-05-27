using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableAnimation : MonoBehaviour
{
    public Image litTable;
    public float animationTime;
    public float minimumAlpha;
    public float maximumAlpha;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(changeOpacity());
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    IEnumerator changeOpacity()
    {
        
        float randomAlpha = Random.Range(minimumAlpha, maximumAlpha);
        litTable.GetComponent<CanvasGroup>().alpha = randomAlpha;
        yield return new WaitForSeconds(animationTime);
        StartCoroutine(changeOpacity());
    }
}
