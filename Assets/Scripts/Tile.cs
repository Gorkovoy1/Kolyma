using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerClickHandler
{
    Image image;

    public bool tileRevealed = false;
    public bool special = false;
    public Sprite originalSprite;
    public Sprite hiddenSprite;

    public int cardValue;

    public void OnPointerClick(PointerEventData eventData)
    {
        print ("You pressed on tile");
        if (tileRevealed)
        {
            hideCard();
        }
        else
        {
            revealCard();
        }
    }

    public void hideCard()
    {
        GetComponent<Image>().sprite = hiddenSprite;
        tileRevealed = false;
    }

    public void revealCard()
    {
        GetComponent<Image>().sprite = originalSprite;
        tileRevealed = true;
    }

    void Start()
    {
        hideCard();
    }

}
