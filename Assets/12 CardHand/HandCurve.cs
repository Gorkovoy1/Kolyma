using UnityEngine;

public class HandCurve : MonoBehaviour
{
    public RectTransform[] cards;          // Array of cards in the hand
    public AnimationCurve positionCurve;   // Curve for vertical position offsets
    public AnimationCurve rotationCurve;   // Curve for rotation offsets
    public float spacing = 100f;           // Base horizontal spacing between cards
    public float curveHeightMultiplier = 50f; // Scale for vertical offset
    public float rotationMultiplier = 15f; // Scale for rotation
    public Vector3 centerOffset;           // Center offset for the hand of cards

    void ArrangeCards()
    {
        int cardCount = cards.Length;
        if (cardCount == 0) return;

        // Middle index for symmetry
        float middleIndex = (cardCount - 1) / 2f;

        for (int i = 0; i < cardCount; i++)
        {
            // Normalize position relative to the middle index
            float normalizedPosition = (i - middleIndex) / middleIndex;

            // Evaluate curve values
            float curveOffsetY = positionCurve.Evaluate(normalizedPosition); // Vertical offset from curve
            float curveRotation = rotationCurve.Evaluate(normalizedPosition); // Rotation from curve

            // Calculate position
            Vector3 offset = new Vector3(i * spacing - middleIndex * spacing, curveOffsetY * curveHeightMultiplier, 0) + centerOffset;

            // Apply position and rotation
            cards[i].anchoredPosition = offset;
            cards[i].localEulerAngles = new Vector3(0, 0, curveRotation * rotationMultiplier);
        }
    }

    void Start()
    {
        ArrangeCards(); // Arrange the cards when the game starts
    }

    void Update()
    {
        ArrangeCards(); // Arrange the cards when the game starts
    }
}