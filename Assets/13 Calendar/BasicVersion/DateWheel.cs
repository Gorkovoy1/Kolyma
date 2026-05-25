using System.Collections;
using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;

public class DateWheel : MonoBehaviour
{
    public enum WheelType { DayOfWeek, Month, Day, Year }

    public RectTransform container;
    public float slotWidth;

    public bool isSpinning;

    private Vector2 startPos;
    private Vector2 targetPos;

    public bool test;

    public int daysToAdvance;

    public DateTime startDate;

    public List<TextMeshProUGUI> items;

    public WheelType type;

    public float speed;

    void Start()
    {
        startPos = container.anchoredPosition;
        targetPos = startPos;
    }

    public void InitializeDate()
    {
        

        if(type == WheelType.DayOfWeek)
        {
            for (int i = 0; i < items.Count; i++)
            {
                DateTime d = startDate.AddDays(i);

                items[i].text = GetText(d).ToUpper();
            }
        }
        else if(type == WheelType.Day)
        {
            for (int i = 0; i < items.Count; i++)
            {
                int value = startDate.Day + i;

                // wrap around 1–31
                if (value > DateTime.DaysInMonth(startDate.Year, startDate.Month))
                    value -= DateTime.DaysInMonth(startDate.Year, startDate.Month);

                items[i].text = value.ToString();
            }
        }
        else if(type == WheelType.Month)
        {
            for (int i = 0; i < items.Count; i++)
            {
                int value = startDate.Month + i;

                if (value > 12)
                    value -= 12;

                DateTime d = new DateTime(startDate.Year, value, 1);

                items[i].text = d.ToString("MMMM").ToUpper();
            }
        }
        else if(type == WheelType.Year)
        {
            for(int i = 0; i < items.Count; i++)
            {
                int value = startDate.Year + i;
                items[i].text = value.ToString();
            }
        }
        
    }

    string GetText(DateTime date)
    {
        return type switch
        {
            WheelType.DayOfWeek => date.DayOfWeek.ToString(),
            WheelType.Month => date.ToString("MMMM").ToUpper(),
            WheelType.Day => date.Day.ToString().ToUpper(),
            WheelType.Year => date.Year.ToString().ToUpper(),
            _ => ""
        };
    }

    void Update()
    {
        container.anchoredPosition = Vector2.Lerp(
            container.anchoredPosition,
            targetPos,
            Time.deltaTime * speed
        );

        // snap when close
        if (isSpinning &&
            Vector2.Distance(container.anchoredPosition, targetPos) < 1f)
        {
            container.anchoredPosition = targetPos;
            isSpinning = false;
        }

        if (test)
        {
            test = false;
            SpinSlots(daysToAdvance);
        }
    }

    public void SpinSlots(int slotNumber)
    {
        if (isSpinning) return;

        isSpinning = true;

        targetPos += Vector2.left * slotWidth * slotNumber;
    }
}