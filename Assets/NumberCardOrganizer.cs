using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberCardOrganizer : MonoBehaviour
{
    public float OffsetPerValue;

    public List<GameObject> cards;

    public Vector2 startPos = new Vector2(0f, 0f);

    public bool NegativeZone;
    public NumberCardOrganizer PairedZone;

    private void Start()
    {
        cards = new List<GameObject>();
    }

    public void Reorganize()
    {
        if(NegativeZone)
        {
            if(PairedZone.transform.childCount > 0)
            {
                ReorganizeNegative(PairedZone.transform.GetChild(PairedZone.transform.childCount - 1));
            }
            else
            {
                ReorganizeCards(startPos);
            }
        }
        else
        {
            ReorganizeCards(startPos);
        }
    }

    public void ReorganizeCards(Vector2 startPos)
    {
            float totalValue = 0f;
        for (int i = 0; i < cards.Count; i++)
        {
            totalValue += Mathf.Abs(cards[i].GetComponent<DisplayCard>().value);
        }
        Vector2 lastCardPos = startPos;

        for (int i = 0; i < cards.Count; i++)
        {
            int offsetValue = Mathf.Abs(cards[i].GetComponent<DisplayCard>().value);
            Vector2 cardPos = lastCardPos - new Vector2(offsetValue * OffsetPerValue, 0f);
            cards[i].transform.localPosition = cardPos;
            lastCardPos = cards[i].transform.localPosition;
            cards[i].transform.SetAsFirstSibling();
        }
    }

    public void ReorganizeNegative(Transform positiveRightMostCard)
    {
        if(cards.Count > 0)
        {
            Vector2 rightMostCardPos = positiveRightMostCard.transform.localPosition;
            DisplayCard card = positiveRightMostCard.gameObject.GetComponent<DisplayCard>();
            Vector2 negativeStartPos = new Vector2(rightMostCardPos.x, 0) + new Vector2((card.value * OffsetPerValue) - (10 * OffsetPerValue) + Mathf.Abs(cards[0].GetComponent<DisplayCard>().value) * OffsetPerValue, 0);
            ReorganizeCards(negativeStartPos);
        }
    }

    public void PlaceCard(GameObject newCard)
    {
        newCard.transform.SetParent(transform);
        newCard.GetComponent<DisplayCard>().NumberCardOrganizer = this;
        cards.Add(newCard);
        if(NegativeZone)
        {
            PairedZone.Reorganize();
            Reorganize();
        }
        else
        {
            Reorganize();
            PairedZone.Reorganize();
        }
    }

    public void RemoveCard(GameObject removedCard)
    {
        cards.Remove(removedCard);
        removedCard.GetComponent<DisplayCard>().NumberCardOrganizer = null; 
        if (NegativeZone)
        {
            PairedZone.Reorganize();
            Reorganize();
        }
        else
        {
            Reorganize();
            PairedZone.Reorganize();
        }
    }
}

