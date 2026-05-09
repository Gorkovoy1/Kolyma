using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelManager : MonoBehaviour
{
    public GameObject inventoryUI;
    public GameObject bettingPanel;
    public GameObject inventorySlotPanel;
    public GameObject hudPanel;

    public UIState currentState;

    // Start is called before the first frame update
    void Start()
    {
        SetState(UIState.Minimized);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetState(UIState newState)
    {
        currentState = newState;

        // Turn everything off first
        inventoryUI.SetActive(false);
        bettingPanel.SetActive(false);
        inventorySlotPanel.SetActive(false);
        hudPanel.SetActive(false);

        // Turn on what this state needs
        switch (currentState)
        {
            case UIState.Minimized:
                hudPanel.SetActive(true);
                break;

            case UIState.Bet:
                bettingPanel.SetActive(true);
                inventorySlotPanel.SetActive(true);
                break;

            case UIState.Inventory:
                inventoryUI.SetActive(true);
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
    Minimized
}