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
        //reset index
        index = 0;

        //assign all date objects
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
            ShiftCalendar(); //check for last date in month, then shift calendar
        }

        if (dates[index].GetComponent<DateInfo>().endOfWeek)
        {
            UpdateGauge();
        }

        //advance
        index++;
        yield return new WaitForSeconds(0.2f);

        
        //advance until weekend
        if (!dates[index].GetComponent<DateInfo>().weekend)
        {
            StartCoroutine(Advance());
        }
        
        
    }

    void UpdateGauge()
    {
        GaugeController.instance.AddCold();
        GaugeController.instance.AddHunger();
        GaugeController.instance.AddWeakness();
    }

    void ShiftCalendar()
    {
        
        
            //shift month to the left
            CalendarObj.transform.position = new Vector2(CalendarObj.transform.position.x + shift, CalendarObj.transform.position.y);

            if (monthText.text == "December")
            {
                monthText.text = "January";
            }
            else if (monthText.text == "January")
            {
                monthText.text = "February";
            }

            Debug.Log(monthText.text);

        

    }
}
