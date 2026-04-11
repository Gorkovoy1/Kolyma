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
    public float minIntensity = 1.5f;
    public float maxIntensity = 4f;
    public float flickerSpeed = 2f;

    private Material mat;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(changeOpacity());
        mat = litTable.material;
        //StartCoroutine(ChangeGlow());
    }

    IEnumerator ChangeGlow()
    {
        while (true)
        {
            float intensity = Random.Range(minIntensity, maxIntensity);

            Color baseColor = Color.white;
            mat.SetColor("_Color", baseColor * intensity);

            yield return new WaitForSeconds(animationTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);

        Color baseColor = new Color(0.0767f, 0.0767f, 0.0767f); 
        mat.SetColor("_Color", baseColor * intensity);
    }

    IEnumerator changeOpacity()
    {
        
        float randomAlpha = Random.Range(minimumAlpha, maximumAlpha);
        litTable.GetComponent<CanvasGroup>().alpha = randomAlpha;
        yield return new WaitForSeconds(animationTime);
        StartCoroutine(changeOpacity());
    }
}
