using UnityEngine;

public class HandCurve : MonoBehaviour
{
    public Transform[] cards; // Assign your card GameObjects in the inspector
    public float radius = 2.0f; // Radius of the arc
    public float maxAngle = 60f; // Total angle covered by the arc (in degrees)

    void Start()
    {
        
    }

    void Update()
    {
        ArrangeCards();
    }

    public void ArrangeCards()
    {
        int cardCount = cards.Length;
        if (cardCount == 0) return;

        float angleStep = 0;
        if (cardCount > 1)
            angleStep = maxAngle / (cardCount - 1);

        float startAngle = -maxAngle / 2;

        for (int i = 0; i < cardCount; i++)
        {
            float angle = startAngle + angleStep * i;
            float rad = Mathf.Deg2Rad * angle;

            // Position the card along the arc
            Vector3 pos = new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0) * radius;
            cards[i].localPosition = pos;

            // Rotate the card to face outward
            cards[i].localRotation = Quaternion.Euler(0, 0, -angle);
        }
    }
}