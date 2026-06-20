using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class RecordController : MonoBehaviour
{
    public GameObject record;
    public GameObject recordTarget;
    public float snapDistance;
    public bool snapped;

    public GameObject creditsPanel;
    public float speed;

    public bool triggerCredits = false;

    private CanvasGroup canvasGroup;
    public float fadeDuration = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();
        StartCoroutine(FadeInRecord());
    }

    IEnumerator FadeInRecord()
    {
        canvasGroup.alpha = 0f;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            canvasGroup.alpha =
                Mathf.Lerp(0f, 1f, timer / fadeDuration);

            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    public IEnumerator FadeOutRecord()
    {
        canvasGroup.alpha = 1f;

        float timer = 0f;

        while (timer < 0.15f)
        {
            timer += Time.deltaTime;

            canvasGroup.alpha =
                Mathf.Lerp(1f, 0f, timer / 0.15f);

            yield return null;
        }

        canvasGroup.alpha = 0f;

        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(record.transform.position + "+" + recordTarget.transform.position);


        if (snapped)
        {
            if (creditsPanel.GetComponent<RectTransform>().anchoredPosition.y > 0)
                Debug.Log(creditsPanel.GetComponent<RectTransform>().anchoredPosition);

            if(triggerCredits)
                creditsPanel.SetActive(true);
            return;
        }
            

        float dist = Vector3.Distance(record.transform.position, recordTarget.transform.position);

        //Debug.Log(dist);

        if (dist < snapDistance)
        {
            record.transform.position = recordTarget.transform.position;
            StartCoroutine(OnSnap());
        }
    }

    IEnumerator OnSnap()
    {
        yield return new WaitForSeconds(0.3f);
        snapped = true;
        triggerCredits = true;
    }

    
}
