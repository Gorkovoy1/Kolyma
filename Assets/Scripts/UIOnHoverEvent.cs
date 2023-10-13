using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIOnHoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Vector3 cachedScale;
    Vector3 cachedPosition;
    private Canvas tempCanvas;
    private GraphicRaycaster tempRaycaster;
    public CardGameManager gm;
    Vector3 originalPosition;

    

    void Start()
    {
        gm = GameObject.Find("Game Manager").GetComponent<CardGameManager>();
        cachedScale = transform.localScale;
        cachedPosition = transform.localPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //x = 40;

        tempCanvas = gameObject.AddComponent<Canvas>();
        tempCanvas.overrideSorting = true;
        tempCanvas.sortingOrder = 1;
        tempRaycaster = gameObject.AddComponent<GraphicRaycaster>();

        transform.localScale = new Vector3(1f, 1f, 0.5f);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 90, transform.localPosition.z);
        //transform.position += Vector3.up;
        //transform.localPosition = new Vector3(transform.localPosition.x, 200, transform.localPosition.z);
        //i think the issue is that its parented

        //Debug.Log("enter");
        originalPosition = gm.playerHandTransform.position;
        gm.playerHandTransform.position += Vector3.up * 140f;
    }


    //fix on click position messing up and not registering as on pointer exit

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(tempRaycaster);
        Destroy(tempCanvas);
        

        transform.localScale = cachedScale;
        gm.playerHandTransform.position = new Vector3(450, -20, 0);
        //transform.localPosition = cachedPosition;
        
        //Debug.Log("exit");
    }

    
    
}
