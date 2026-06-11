using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RecordSpinner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(this.GetComponentInParent<RecordController>().snapped)
            this.transform.Rotate(0f, 0f, speed * Time.deltaTime);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (this.GetComponentInParent<RecordController>().snapped)
            return;

        GetComponent<RectTransform>().anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}
