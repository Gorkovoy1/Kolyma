using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandFanController : MonoBehaviour
{
    public bool fanHand;
    private RectTransform rect;
    private float yPos;
    public bool dragging;

    public bool seeBoard = false;

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        yPos = rect.anchoredPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(TurnManager.instance.isPlayerTurn && !seeBoard)
        {
            fanHand = true;
        }
        else if(TurnManager.instance.isPlayerTurn && seeBoard)
        {
            fanHand = false;
        }
        else if(!TurnManager.instance.isPlayerTurn)
        {
            fanHand = false;
        }

        if(!fanHand)
        {
            rect.sizeDelta = new Vector2(50f, rect.sizeDelta.y);
            rect.anchoredPosition = new Vector2(-350f, yPos - 50f);
            //make cards smaller?
        }
        else
        {
            rect.sizeDelta = new Vector2(400f, rect.sizeDelta.y);
            rect.anchoredPosition = new Vector2(0f, yPos);
            
        }
    }

    public void ToggleSee()
    {
        seeBoard = !seeBoard;
    }
}
