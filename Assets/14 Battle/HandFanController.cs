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

    public GameObject cardSelectionBlocker;

    public bool hoverable;
    public float hoverDelay;


    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        yPos = rect.anchoredPosition.y;
        hoverable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            StartCoroutine(ToggleSee());
        }

        if(cardSelectionBlocker != null && cardSelectionBlocker.gameObject.activeSelf)
        {
            seeBoard = true;
        }


        if(TurnManager.instance.isPlayerTurn && !seeBoard)
        {
            if(!fanHand)
                StartCoroutine(ToggleHover());
            fanHand = true;
        }
        else if(TurnManager.instance.isPlayerTurn && seeBoard)
        {
            if(fanHand)
                StartCoroutine(ToggleHover());
            fanHand = false;
        }
        else if(!TurnManager.instance.isPlayerTurn)
        {
            if (fanHand)
                StartCoroutine(ToggleHover());
            fanHand = false;
        }
        
        

        if(!fanHand)
        {
            rect.sizeDelta = new Vector2(50f, rect.sizeDelta.y);
            rect.anchoredPosition = new Vector2(-500f, yPos - 50f);
            //make cards smaller?
        }
        else
        {
            rect.sizeDelta = new Vector2(400f, rect.sizeDelta.y);
            rect.anchoredPosition = new Vector2(0f, yPos);
            
        }
    }

    IEnumerator ToggleSee()
    {
        hoverable = false;
        seeBoard = !seeBoard;
        yield return new WaitForSeconds(hoverDelay);
        hoverable = true;
    }

    IEnumerator ToggleHover()
    {
        hoverable = false;
        yield return new WaitForSeconds(hoverDelay);
        hoverable = true;
    }
}
