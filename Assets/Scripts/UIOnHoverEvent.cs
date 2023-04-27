using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIOnHoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Vector3 cachedScale;
    //Vector3 cachedPosition;
    private Canvas tempCanvas;
    private GraphicRaycaster tempRaycaster;

    //public int x;

    void Start()
    {
        cachedScale = transform.localScale;
        //cachedPosition = transform.localPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //x = 40;

        tempCanvas = gameObject.AddComponent<Canvas>();
        tempCanvas.overrideSorting = true;
        tempCanvas.sortingOrder = 1;
        tempRaycaster = gameObject.AddComponent<GraphicRaycaster>();

        transform.localScale = new Vector3(0.27f, 0.27f, 0.5f);
        //transform.position += Vector3.up;
        //transform.localPosition = new Vector3(transform.localPosition.x, 200, transform.localPosition.z);
        //i think the issue is that its parented

        //Debug.Log("enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(tempRaycaster);
        Destroy(tempCanvas);
        

        transform.localScale = cachedScale;
        
        //Debug.Log("exit");
    }
}
