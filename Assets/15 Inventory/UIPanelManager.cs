using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPanelManager : MonoBehaviour
{
    public GameObject inventoryUI;
    public GameObject bettingPanel;
    public GameObject inventorySlotPanel;
    public GameObject hudPanel;
    public GameObject betSlotsPanel;
    public GameObject winningsPanel;

    public UIState currentState;

    public static UIPanelManager instance;

    public Transform AnimTextParent;

    public int weekNumber = 1;
    [SerializeField] TextMeshProUGUI weekText;

    // Start is called before the first frame update
    void Start()
    {
        instance = InventoryManager.instance.gameObject.GetComponent<UIPanelManager>();
        //SetState(UIState.Minimized);
        SetState(currentState);
    }

    // Update is called once per frame
    void Update()
    {
        weekText.text = "Week " + weekNumber;
    }

    public void SetState(UIState newState)
    {
        currentState = newState;

        // Turn everything off first
        inventoryUI.SetActive(false); //for management
        bettingPanel.SetActive(false);
        inventorySlotPanel.SetActive(false);
        hudPanel.SetActive(false);
        betSlotsPanel.SetActive(false);
        winningsPanel.SetActive(false);

        // Turn on what this state needs
        switch (currentState)
        {
            case UIState.Minimized:
                hudPanel.SetActive(true);
                break;

            case UIState.Bet:
                bettingPanel.SetActive(true);
                betSlotsPanel.SetActive(true);
                inventorySlotPanel.SetActive(true);
                break;

            case UIState.Inventory:
                inventoryUI.SetActive(true);
                inventorySlotPanel.SetActive(true);
                break;
            case UIState.Inactive:
                break;
            case UIState.Winnings:
                bettingPanel.SetActive(true);
                winningsPanel.SetActive(true);
                inventorySlotPanel.SetActive(true);
                break;

        }
    }

    public void BackToHUD()
    {
        SetState(UIState.Minimized);
    }

    public void OpenInvManager()
    {
        SetState(UIState.Inventory);
    }
}

public enum UIState
{
    Inventory,
    Bet,
    Minimized,
    Inactive,
    Winnings
}