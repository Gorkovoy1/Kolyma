using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarkerController : MonoBehaviour
{
    public int index;
    public List<GameObject> dates;
    public GameObject months;
    public GameObject CalendarObj;

    public int shift;
    public TextMeshProUGUI monthText;

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
        if (dates[index].GetComponent<DateInfo>().lastDateInMonth)
        {
            //shift month to the left
            CalendarObj.transform.position = new Vector2(CalendarObj.transform.position.x + shift, CalendarObj.transform.position.y);

            if(monthText.text == "December")
            {
                monthText.text = "January";
            }
            else if (monthText.text == "January")
            {
                monthText.text = "February";
            }

            Debug.Log(monthText.text);

        }


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
