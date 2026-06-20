using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using AK.Wwise;

public class RecordSpinner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public float speed;
    public bool startMusic = false;
    public uint musicId;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(this.GetComponentInParent<RecordController>().snapped)
        {
            this.transform.Rotate(0f, 0f, speed * Time.deltaTime);

            if (!startMusic)
            {
                startMusic = true;
                musicId = AkSoundEngine.PostEvent("Play_Prologue_Record", this.gameObject);
            }
            
        }
            
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetSiblingIndex(4);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.SetSiblingIndex(4);

        if (this.GetComponentInParent<RecordController>().snapped)
            return;

        GetComponent<RectTransform>().anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetSiblingIndex(2);
    }
}
