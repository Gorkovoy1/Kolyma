using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberCardOrganizer : MonoBehaviour
{
    public float OffsetPerValue;

    public List<GameObject> cards;

    public Vector2 startPos = new Vector2(-100f, 0f);

    private void Start()
    {
        cards = new List<GameObject>();
    }

    public void Reorganize()
    {
        float totalValue = 0f;
        for (int i = 0; i < cards.Count; i++)
        {
            totalValue += Mathf.Abs(cards[i].GetComponent<DisplayCard>().value);
        }
        float nextCardOffset = 0f;
        Vector2 lastCardPos = startPos;
        //Vector2 lastCardPos = new Vector2(totalValue / 2 * -OffsetPerValue, 0f);
        for(int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.localPosition = new Vector2(lastCardPos.x + nextCardOffset, 0f);
            DisplayCard displayCard = cards[i].GetComponent<DisplayCard>();
            int value = Mathf.Abs(displayCard.value);
            Debug.Log("Value:" + value);
            nextCardOffset = value * OffsetPerValue;
            lastCardPos = cards[i].transform.localPosition;
        }
    }

    public void PlaceCard(GameObject newCard)
    {
        newCard.transform.SetParent(transform);
        newCard.GetComponent<DisplayCard>().NumberCardOrganizer = this;
        cards.Add(newCard);
        Reorganize();
    }

    public void RemoveCard(GameObject removedCard)
    {
        cards.Remove(removedCard);
        removedCard.GetComponent<DisplayCard>().NumberCardOrganizer = null;
        Reorganize();
    }
}
