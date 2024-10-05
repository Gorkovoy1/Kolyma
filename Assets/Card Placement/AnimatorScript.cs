using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatorScript : MonoBehaviour
{
    public RectTransform target;       // Target UI element's RectTransform
    public Image cardImage;            // Image to animate
    public float speed = 1f;       // Speed of movement in UI units (e.g., pixels per second)
    private RectTransform cardRectTransform; // RectTransform of the cardImage
    private RectTransform targetRectTransform;

    // Start is called before the first frame update
    void Start()
    {
        // Get the RectTransform of the card image
        cardRectTransform = cardImage.GetComponent<RectTransform>();

        targetRectTransform = target.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(cardRectTransform.position.x != targetRectTransform.position.x)
        cardRectTransform.position = new Vector2(cardRectTransform.position.x -0.01f, cardRectTransform.position.y);
        if(cardRectTransform.position.y != targetRectTransform.position.y)
        cardRectTransform.position = new Vector2(cardRectTransform.position.x, cardRectTransform.position.y -0.01f);
    }
}