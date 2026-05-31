using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public List<GameObject> breadList;
    public List<GameObject> drinkList;
    public List<GameObject> clothesList;
    public List<GameObject> otherList;

    public int breadIndex = 0;
    public int drinkIndex = 0;
    public int clothesIndex = 0;
    public int otherIndex = 0;

    public static InventoryManager instance;

    public GameObject slotPrefab;

    public GameObject[] betSlotArray;

    public GameObject consumeArea;

    public UIPanelManager uiPanelManager;

    [Header("Pot")]
    public int moneyInPot;
    public GameObject[] itemsInPot;
    public bool showWinnings;
    public bool winningsShown;

    public Transform winningsPanel;

    public GameObject claimMoneyButton;
    

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        uiPanelManager = GetComponent<UIPanelManager>();

        //Debug.Log("Inventory Awake: " + gameObject.name +  " ID: " + gameObject.GetInstanceID());
    }

    // Start is called before the first frame update
    void Start()
    {
        itemsInPot = new GameObject[2];
        showWinnings = false;
        winningsShown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(showWinnings && !winningsShown)
        {
            winningsShown = true;
            InventoryManager.instance.uiPanelManager.SetState(UIState.Winnings);
            ShowWinnings();
        }
    }

    public void AddNewItem(GameObject objToAdd, List<GameObject> list, int index)
    {
        foreach(GameObject g in list)
        {
            if(g.transform.childCount == 0)
            {
                index = list.IndexOf(g);
                break;
            }
            else
            {
                index = -1;
            }
        }
        if(index == -1)
        {
            //add one more slot
            GameObject newSlot = Instantiate(slotPrefab, list[0].transform.parent);
            list.Add(newSlot);
            index = list.IndexOf(newSlot);
        }


        if(objToAdd.transform.parent.name == "ItemPanel")
        {
            GameObject newObj = Instantiate(objToAdd, list[index].transform);
            newObj.transform.localPosition = Vector3.zero;
            objToAdd.GetComponent<Button>().interactable = false;
            
        }
        else
        {
            objToAdd.transform.SetParent(list[index].transform);
            objToAdd.transform.localPosition = Vector3.zero;
        }
            
    }

    public void LockPot(int money, GameObject objOne, GameObject objTwo)
    {
        moneyInPot = money;
        itemsInPot[0] = objOne;
        itemsInPot[1] = objTwo;

        //reset the bet slots
        betSlotArray[0].GetComponentInChildren<TextMeshProUGUI>().text = "0";
        foreach (Transform child in betSlotArray[1].transform)
            Destroy(child.gameObject);
        foreach (Transform child in betSlotArray[2].transform)
            Destroy(child.gameObject);
    }

    public void ShowWinnings()
    {
        //add money
        GameObject moneyButton = Instantiate(claimMoneyButton, uiPanelManager.winningsPanel.transform);
        moneyButton.GetComponent<ClaimMoney>().SetMoney(moneyInPot);

        //show objects
        foreach(GameObject item in itemsInPot)
        {
            if(item != null)
            {
                Instantiate(item, uiPanelManager.winningsPanel.transform);
                Instantiate(item, uiPanelManager.winningsPanel.transform);
            }
        }
    }

}
