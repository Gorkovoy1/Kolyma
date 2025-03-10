using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonthHandler : MonoBehaviour
{
    public GameObject[] week1;
    public GameObject[] week2;
    public GameObject[] week3;
    public GameObject[] week4;
    public GameObject[] week5;
    public GameObject[][] month;
    public int date;
    public int max;
    public int min;

    // Start is called before the first frame update
    void Start()
    {
        month = new GameObject[5][];
        month[0] = week1;
        month[1] = week2;   
        month[2] = week3;   
        month[3] = week4;
        month[4] = week5;


        date = min;

        foreach(var week in month)
        {
            foreach(var day in week)
            {
                day.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "" + date;

                date++;

                if(date > max)
                {
                    date = 1;

                    if(max == 30)
                    {
                        max = 31;
                    }
                    else
                    {
                        max = 30;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
