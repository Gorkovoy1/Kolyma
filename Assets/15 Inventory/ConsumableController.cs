using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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

    public bool betting = false;

    private GameObject consumeArea;

    private Transform originalParent;
    public Vector2 originalPosition;

    private CanvasGroup cg;

    public GameObject[] betSlots;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "BetSimulation" || SceneManager.GetActiveScene().name == "4 Bet")
        {
            betting = true;
            betSlots = InventoryManager.instance.betSlotArray;
        }
        else
        {
            betting = false;
        }

        cg = GetComponent<CanvasGroup>();

        if(betting)
            originalPosition = transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!betting)
        {
            //enable the consume area
            consumeArea = transform.parent.parent.Find("ConsumeArea").gameObject;
            consumeArea.SetActive(true);
        }
        else
        {
            cg.blocksRaycasts = false;
            originalParent = transform.parent;

            transform.SetParent(canvas.transform); // bring to top
        }
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!betting)
        {
            //if dragged into consumable square, delete self and replenish corresponding value
            if (RectTransformUtility.RectangleContainsScreenPoint(consumeArea.GetComponent<RectTransform>(), Input.mousePosition, eventData.pressEventCamera))
            {
                Replenish();
            }

            //disable consumearea back
            consumeArea.SetActive(false);
        }
        else
        {
            cg.blocksRaycasts = true;

            // If not placed in slot → snap back
            if (transform.parent == canvas.transform)
            {
                transform.SetParent(originalParent);
                this.transform.localPosition = Vector3.zero;
            }
        }
        
        
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

    public void ItemClicked()
    {
        if(betting)
        {
            if(this.transform.parent.CompareTag("InvSlot"))
            {
                foreach(GameObject g in betSlots)
                {
                    if(g.transform.childCount == 0)
                    {
                        this.transform.SetParent(g.transform);
                        this.transform.localPosition = Vector3.zero;
                        break;
                    }
                }
                
                
            }
            else if(this.rectTransform.parent.CompareTag("BetSlot"))
            {
                if(this.gameObject.CompareTag("Bread"))
                {
                    InventoryManager.instance.AddNewItem(this.gameObject, InventoryManager.instance.breadList, InventoryManager.instance.breadIndex);
                }
                else if(this.gameObject.CompareTag("Drink"))
                {
                    InventoryManager.instance.AddNewItem(this.gameObject, InventoryManager.instance.drinkList, InventoryManager.instance.drinkIndex);
                }
                else if(this.gameObject.CompareTag("Clothes"))
                {
                    InventoryManager.instance.AddNewItem(this.gameObject, InventoryManager.instance.clothesList, InventoryManager.instance.clothesIndex);
                }
                else if(this.gameObject.CompareTag("Other"))
                {
                    InventoryManager.instance.AddNewItem(this.gameObject, InventoryManager.instance.otherList, InventoryManager.instance.otherIndex);
                }
            }
        }
    }

    
}

public enum ConsumableType
{
    Hunger,
    Cold,
    Weakness
}
