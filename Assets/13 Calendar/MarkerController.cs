using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkerController : MonoBehaviour
{
    public int index;
    public List<GameObject> dates;
    public GameObject months;

    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        dates = new List<GameObject>();

        Transform[] children = months.transform.GetComponentsInChildren<Transform>();

        foreach(var child in children)
        {
            if(child.gameObject.TryGetComponent(out DateInfo otherIsDate))
            {
                dates.Add(otherIsDate.gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = dates[index].transform.position;
    }

    public void GoNext()
    {
        if(index == dates.Count-1)
        {
            Debug.Log("last date");
        }
        else
        {
            StartCoroutine(Advance());
        }
    }

    IEnumerator Advance()
    {
        index++;
        yield return new WaitForSeconds(0.2f);
        if (!dates[index].GetComponent<DateInfo>().weekend)
        {
            StartCoroutine(Advance());
        }
        
        
        /*
        for(int i = 0; i < 7; i++)
        {
            index++;
            yield return new WaitForSeconds(0.2f);
        }
        */
    }
}
