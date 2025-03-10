using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonthHandler : MonoBehaviour
{

    public GameObject[][] totalMonths;
    public GameObject[] month1;
    public GameObject[] month2;
    public GameObject[] month3;
    public int date;
    public int max;
    public int min;

    // Start is called before the first frame update
    void Start()
    {
        //attach 3 months to total months
        totalMonths = new GameObject[3][];
        totalMonths[0] = month1;
        totalMonths[1] = month2;
        totalMonths[2] = month3;
        //attach 4 weeks to each month
        

        date = min;

        foreach(var month in totalMonths)
        {
            foreach (var week in month)
            {
                foreach (Transform child in week.transform)
                {
                    child.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "" + date;

                    date++;

                    if (date > max)
                    {
                        date = 1;

                        if (max == 30)
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
