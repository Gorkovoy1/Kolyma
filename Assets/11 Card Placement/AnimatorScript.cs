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


    public float tiltAngle;
    public Quaternion targetRotation;

    // Start is called before the first frame update
    void Start()
    {
        // Get the RectTransform of the card image
        cardRectTransform = cardImage.GetComponent<RectTransform>();

        targetRectTransform = target.GetComponent<RectTransform>();

        target.gameObject.GetComponent<CardPlace>().correspondingImage = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Destroy(this.gameObject);
        }
        else
        {




            float tiltAngle = 0;
            float rotationSpeed = 0.5f;  // Speed for rotation interpolation

            // Current world position of the card
            Vector3 cardWorldPosition = cardRectTransform.position;



            // Target world position
            Vector3 targetWorldPosition = targetRectTransform.position;

            // Smoothly interpolate the card's position towards the target position
            cardRectTransform.position = Vector3.Lerp(cardWorldPosition, targetWorldPosition, Time.deltaTime * lerpSpeed);

            float deltaX = targetWorldPosition.x - cardWorldPosition.x;

            //Debug.Log(deltaX);




            if (targetRectTransform.gameObject.GetComponent<NumberStats>().positive)
            {
                tiltAngle = 0f; // Default tilt angle

                if (deltaX > 0)
                {
                    //Debug.Log("tilt right");
                    tiltAngle = -deltaX * rotationSpeed; // Tilt right (negative Z rotation)
                }
                else if (deltaX < 0)
                {
                    //Debug.Log("tilt left");
                    tiltAngle = deltaX * -rotationSpeed; // Tilt left (positive Z rotation)
                }
                else
                {
                    //Debug.Log("no tilt");
                }

                // Create target rotation as a Quaternion (avoids issues with Euler angles)
                targetRotation = Quaternion.Euler(0, 0, tiltAngle);

                // Smoothly interpolate using Lerp
                cardRectTransform.rotation = Quaternion.Lerp(cardRectTransform.rotation, targetRotation, Time.deltaTime * 8f);
            }

            else if (targetRectTransform.gameObject.GetComponent<NumberStats>().negative)
            {
                cardRectTransform.localScale = new Vector3(0.081f, 0.081f, 0.081f);

                tiltAngle = 0f; // Default tilt angle

                if (deltaX > 0)
                {
                    //Debug.Log("tilt right");
                    tiltAngle = -deltaX * rotationSpeed; // Tilt right (negative Z rotation)
                }
                else if (deltaX < 0)
                {
                    //Debug.Log("tilt left");
                    tiltAngle = deltaX * -rotationSpeed; // Tilt left (positive Z rotation)
                }
                else
                {
                    //Debug.Log("no tilt");
                }

                // Create target rotation as a Quaternion (avoids issues with Euler angles)
                targetRotation = Quaternion.Euler(0, 0, tiltAngle);

                // Smoothly interpolate using Lerp
                cardRectTransform.rotation = Quaternion.Lerp(cardRectTransform.rotation, targetRotation, Time.deltaTime * 8f);

            }
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