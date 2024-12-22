using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class CardPlace : MonoBehaviour,
    IDragHandler, IBeginDragHandler, IEndDragHandler

{
    public bool isDragging;
    public Transform parentReturnTo = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log ("OnBeginDrag");

        
            //if(conditionMet == true)
            //{
            parentReturnTo = this.transform.parent;
            
            this.transform.SetParent(parentReturnTo.transform.parent);
            

            GetComponent<CanvasGroup>().blocksRaycasts = false;
            //}
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        

            this.transform.SetParent(parentReturnTo);
            //this.transform.position = new Vector3(0,0,0);

            GetComponent<CanvasGroup>().blocksRaycasts = true;
            
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
        
            //if(conditionMet==true)
            this.transform.position = eventData.position;

    }


}
