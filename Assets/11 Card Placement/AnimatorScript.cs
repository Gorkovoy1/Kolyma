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
        else if(!target.gameObject.GetComponent<CardPlace>().isFlipped)
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

                float currentZ = cardRectTransform.eulerAngles.z;
                float targetZ = tiltAngle; // Or 180f + tiltAngle if card is flipped
                float lerpedZ = Mathf.LerpAngle(currentZ, targetZ, Time.deltaTime * 8f);

                cardRectTransform.rotation = Quaternion.Euler(0f, 0f, lerpedZ);
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

                float currentZ = cardRectTransform.eulerAngles.z;
                float targetZ = tiltAngle; // Or 180f + tiltAngle if card is flipped
                float lerpedZ = Mathf.LerpAngle(currentZ, targetZ, Time.deltaTime * 8f);

                cardRectTransform.rotation = Quaternion.Euler(0f, 0f, lerpedZ);
            }
        }
        else if(target.gameObject.GetComponent<CardPlace>().isFlipped)
        {
            //lerp rotate the card 180

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

                float currentZ = cardRectTransform.eulerAngles.z;
                float targetZ = 180f + tiltAngle; // Or 180f + tiltAngle if card is flipped
                float lerpedZ = Mathf.LerpAngle(currentZ, targetZ, Time.deltaTime * 8f);

                cardRectTransform.rotation = Quaternion.Euler(0f, 0f, lerpedZ);
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

                float currentZ = cardRectTransform.eulerAngles.z;
                float targetZ = 180f + tiltAngle; // Or 180f + tiltAngle if card is flipped
                float lerpedZ = Mathf.LerpAngle(currentZ, targetZ, Time.deltaTime * 8f);

                cardRectTransform.rotation = Quaternion.Euler(0f, 0f, lerpedZ);
            }
        }
        /*
        else if(target.gameObject.GetComponent<CardPlace>().isFlipping)
        {
            StartCoroutine(RotateImage180(0.7f));
        }
        */
    }


    /*
    public IEnumerator RotateImage180(float duration)
    {
        float startAngle = cardRectTransform.eulerAngles.z;
        float endAngle = startAngle + 180f;
        float stepTime = Time.deltaTime;
        int steps = Mathf.CeilToInt(duration / stepTime);

        for (int i = 0; i < steps; i++)
        {
            float t = (float)i / steps;
            float currentAngle = Mathf.LerpAngle(startAngle, endAngle, t);
            cardRectTransform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
            yield return null;
        }

        // Ensure exact end rotation
        cardRectTransform.rotation = Quaternion.Euler(0f, 0f, endAngle);
        target.gameObject.GetComponent<CardPlace>().isFlipping = false;
        target.gameObject.GetComponent<CardPlace>().isFlipped = true;

    }
    */

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