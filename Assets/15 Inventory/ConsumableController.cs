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

    public GameObject consumeArea;

    private Transform originalParent;
    public Vector2 originalPosition;

    private CanvasGroup cg;

    public GameObject[] betSlots;

    private RectTransform parentRect;

    private Vector2 dragOffset;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        parentRect = rectTransform.parent as RectTransform;
        //consumeArea = GameObject.FindWithTag("ConsumeArea");
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(WaitForInventory());
        
    }


    IEnumerator WaitForInventory()
    {
        yield return new WaitUntil(() => InventoryManager.instance != null);
        consumeArea = InventoryManager.instance.consumeArea;
        betSlots = InventoryManager.instance.betSlotArray;

        if (InventoryManager.instance.uiPanelManager.currentState == UIState.Bet)
        {
            betting = true;
            betSlots = InventoryManager.instance.betSlotArray;
        }
        else
        {
            betting = false;
        }

        cg = GetComponent<CanvasGroup>();

        if (betting)
            originalPosition = transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        if(InventoryManager.instance.uiPanelManager.currentState == UIState.Bet || InventoryManager.instance.uiPanelManager.currentState == UIState.Winnings)
        {
            betting = true;
        }
        else
        {
            betting = false;
        }
    }

    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!betting)
        {
            //enable the consume area
            consumeArea.SetActive(true);
            originalPosition = this.transform.position;
            originalParent = this.transform.parent;
        }
        else
        {
            cg.blocksRaycasts = false;
            originalParent = transform.parent;
            originalPosition = transform.position;

            transform.SetParent(canvas.transform); // bring to top
        }

        Vector2 localMousePos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out localMousePos
        );

        dragOffset = rectTransform.anchoredPosition - localMousePos;

    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localMousePos;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out localMousePos))
        {
            rectTransform.anchoredPosition = localMousePos + dragOffset;
        }

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
            else
            {
                //go back to original slot
                this.transform.position = originalPosition;
                this.transform.SetParent(originalParent);
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
        else if(this.transform.parent.name == "ItemPanel") //shop scene, move item to inventory on click
        {
            //SUBTRACT MONEY HERE


            if (this.gameObject.CompareTag("Bread"))
            {
                InventoryManager.instance.AddNewItem(this.gameObject, InventoryManager.instance.breadList, InventoryManager.instance.breadIndex);
            }
            else if (this.gameObject.CompareTag("Drink"))
            {
                InventoryManager.instance.AddNewItem(this.gameObject, InventoryManager.instance.drinkList, InventoryManager.instance.drinkIndex);
            }
            else if (this.gameObject.CompareTag("Clothes"))
            {
                InventoryManager.instance.AddNewItem(this.gameObject, InventoryManager.instance.clothesList, InventoryManager.instance.clothesIndex);
            }
            else if (this.gameObject.CompareTag("Other"))
            {
                InventoryManager.instance.AddNewItem(this.gameObject, InventoryManager.instance.otherList, InventoryManager.instance.otherIndex);
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
