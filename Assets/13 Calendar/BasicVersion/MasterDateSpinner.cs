using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MasterDateSpinner : MonoBehaviour
{
    public int daysToAdvance;
    public List<DateWheel> dateWheels;
    public int year;
    public int month;
    public int day;
    public DateTime startDate;
    public DateWheel monthWheel;
    public DateWheel yearWheel;

    public bool spin = false;
    // Start is called before the first frame update
    void Start()
    {
        startDate = new DateTime(year, month, day);

        foreach(DateWheel dateWheel in dateWheels)
        {
            dateWheel.startDate = startDate;
            dateWheel.InitializeDate();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(spin)
        {
            GaugeController.instance.PassTime(daysToAdvance);

            spin = false;
            foreach(DateWheel dateWheel in dateWheels)
            {
                if(dateWheel.type != DateWheel.WheelType.Month && dateWheel.type != DateWheel.WheelType.Year)
                    dateWheel.SpinSlots(daysToAdvance);
            }


            DateTime current = startDate;

            for (int i = 1; i <= daysToAdvance; i++)
            {
                DateTime next = startDate.AddDays(i);

                if (next.Month != current.Month)
                {
                    SpinMonthWheel();
                }

                current = next;
            }
        }
    }

    void SpinMonthWheel()
    {
        monthWheel.SpinSlots(1);
        if (startDate.Month == 12)
            SpinYearWheel();
    }

    void SpinYearWheel()
    {
        yearWheel.SpinSlots(1);
    }
}
