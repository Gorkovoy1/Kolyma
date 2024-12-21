using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatorScript : MonoBehaviour
{
    public RectTransform target;       // Target UI element's RectTransform
    public GameObject cardImage;            // Image to animate
    public float lerpSpeed = 2f;       // Speed of movement in UI units (e.g., pixels per second)
    private RectTransform cardRectTransform; // RectTransform of the cardImage
    private RectTransform targetRectTransform;


    public float min;
    public float max;
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
        float flipAngle = 0f;
        float tiltAngle = 0;
        float rotationSpeed = 0.5f;  // Speed for rotation interpolation

        // Current world position of the card
        Vector3 cardWorldPosition = cardRectTransform.position;

        Vector3 targetRotation = new Vector3(0,0,0);

        // Target world position
        Vector3 targetWorldPosition = targetRectTransform.position;

        // Smoothly interpolate the card's position towards the target position
        cardRectTransform.position = Vector3.Lerp(cardWorldPosition, targetWorldPosition, Time.deltaTime * lerpSpeed);

        float deltaX = targetWorldPosition.x - cardWorldPosition.x;
        
        Debug.Log(deltaX);

        


        //flipping
        if(cardWorldPosition.y > 400)
        {
            flipAngle = 0f;
            
            //upright horizontal tilt
            if(deltaX > 0)
            {
                Debug.Log("tilt right");
                tiltAngle = -deltaX * rotationSpeed;
            }
            else if(deltaX < 0)
            {
                Debug.Log("tilt left");
                tiltAngle = deltaX * -rotationSpeed;
            }
            else
            {
                Debug.Log("no tilt");
            }
            float combinedRotation = Mathf.Clamp(flipAngle + tiltAngle, min, max);
            targetRotation = new Vector3(0,0, combinedRotation);
            

            cardRectTransform.eulerAngles = Vector3.Lerp(cardRectTransform.eulerAngles, targetRotation, Time.deltaTime * 5f);
        }
        else if(cardWorldPosition.y < 400)
        {
            //flip upside down
            flipAngle = 180f;
            cardRectTransform.eulerAngles = Vector3.Lerp(cardRectTransform.eulerAngles, targetRotation, Time.deltaTime * 5f);
            
            if(deltaX > 0)
            {
                Debug.Log("tilt right");
                tiltAngle = 180 + (-deltaX * rotationSpeed);
            }
            else if(deltaX < 0)
            {
                Debug.Log("tilt left");
                tiltAngle = 180 + (deltaX * -rotationSpeed);
            }
            else
            {
                Debug.Log("no tilt");
            }

            targetRotation = new Vector3(0,0, flipAngle + tiltAngle);

            cardRectTransform.eulerAngles = Vector3.Lerp(cardRectTransform.eulerAngles, targetRotation, Time.deltaTime * 5f);
            
        }
    }


    /*
    void Update()
    {
        float moveSpeed = 1f; // Adjust this value for your desired speed

        // Current world position of the card
        Vector3 cardWorldPosition = cardRectTransform.position;

        // Target world position
        Vector3 targetWorldPosition = targetRectTransform.position;

        // Move the card's position towards the target position
        cardRectTransform.position = Vector3.MoveTowards(cardWorldPosition, targetWorldPosition, moveSpeed);


        //Debug.Log(cardWorldPosition);
        //Debug.Log(targetWorldPosition);
    }
    

    USE THIS FOR DEALING CARDS
    */

}