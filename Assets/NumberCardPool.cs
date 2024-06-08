using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberCardPool : MonoBehaviour
{
    public GameObject CardPrefab;

    public List<int> Values;
    public List<NumberCard> NumberCards;

    private Dictionary<int, NumberCard> NumberCardsDictionary;

    public static NumberCardPool Instance;

    private void Awake()
    {
        NumberCardsDictionary = new Dictionary<int, NumberCard>();
        for(int i = 0; i < Values.Count; i++)
        {
            NumberCardsDictionary.Add(Values[i], NumberCards[i]);
        }
        Instance = this;
    }

    public GameObject GetValue(int value, CharacterInstance owner)
    {
        GameObject newCard = Instantiate(CardPrefab);
        newCard.GetComponent<DisplayCard>().InitNumberCard(NumberCardsDictionary[value], owner);
        return newCard;
    }
}
