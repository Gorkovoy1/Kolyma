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

    // Start is called before the first frame update
    void Start()
    {
        
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
