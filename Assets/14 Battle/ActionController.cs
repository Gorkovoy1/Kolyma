using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActionController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Button swapButton;
    public Button flipButton;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        swapButton.gameObject.SetActive(true);
        flipButton.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        swapButton.gameObject.SetActive(false);
        flipButton.gameObject.SetActive(false);
    }

    public void ActivateSwap()
    {
        foreach (GameObject g in NumberManager.instance.allNumbers)
        {
            g.GetComponent<NumberStats>().selectable = true;
        }

        CardSelectionController.instance.CallButtons("swap", "player");
        NumberManager.instance.playerAction = true;
        TurnManager.instance.playerPlayedCard = true;
        this.gameObject.SetActive(false);
    }

    public void ActivateFlip()
    {
        foreach (GameObject g in NumberManager.instance.allNumbers)
        {
            g.GetComponent<NumberStats>().selectable = true;
        }

        CardSelectionController.instance.CallButtons("flip", "player");
        NumberManager.instance.playerAction = true;
        TurnManager.instance.playerPlayedCard = true;
        this.gameObject.SetActive(false);
    }

}
