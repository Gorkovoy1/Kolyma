using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConsumableController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public ConsumableType consumableType;
    public int replenishValue;

    private RectTransform rectTransform;
    private Canvas canvas;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private GameObject consumeArea;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }
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
        //enable the consume area
        consumeArea = transform.parent.parent.Find("ConsumeArea").gameObject;
        consumeArea.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        StayInArea();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log(rectTransform.anchoredPosition.y);
        Debug.Log(rectTransform.anchoredPosition.x);
        //if dragged into consumable square, delete self and replenish corresponding value
        if (rectTransform.anchoredPosition.x < minX)
        {
            Replenish();
        }
        //if dragged outside of inventory, snap back
        //save position


        //disable consumearea back
        consumeArea.SetActive(false);
    }

    void StayInArea()
    {
        Vector2 pos = rectTransform.anchoredPosition;

        pos.x = Mathf.Clamp(pos.x, -600, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        rectTransform.anchoredPosition = pos;
    }

    void Replenish()
    {
        if(consumableType == ConsumableType.Hunger)
        {
            //add replenish value to master hunger
            Debug.Log("replenished hunger " + replenishValue);
        }
        else if (consumableType == ConsumableType.Cold)
        {
            //add replenish value to master cold
            Debug.Log("replenished cold " + replenishValue);
        }
        else if (consumableType == ConsumableType.Weakness)
        {
            //add replenish value to master weakness
            Debug.Log("replenished weakness " + replenishValue);
        }

        Destroy(this.gameObject);
    }

    
}

public enum ConsumableType
{
    Hunger,
    Cold,
    Weakness
}
