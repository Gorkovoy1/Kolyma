using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TutorialScripts
{
    public class SpecialCardMovement : MonoBehaviour
    {
        public RectTransform target;

        public float lerpSpeed = 5f;
        private RectTransform cardRectTransform;


        public float min;
        public float max;

        void Start()
        {

            cardRectTransform = GetComponent<RectTransform>();


        }

        // Update is called once per frame
        void Update()
        {
            if (!this.target.gameObject.GetComponent<TutorialScripts.CardPlace>().hovering)
            {
                float rotationSpeed = 0.2f;


                Vector3 cardWorldPosition = cardRectTransform.position;



                Vector3 targetWorldPosition = target.position;

                // Smoothly interpolate the card's position towards the target position
                cardRectTransform.position = Vector3.Lerp(cardWorldPosition, targetWorldPosition, Time.deltaTime * lerpSpeed);

                float deltaX = targetWorldPosition.x - cardWorldPosition.x;



                //upright horizontal tilt
                if (deltaX > 0)
                {
                    //Debug.Log("tilt right");
                    cardRectTransform.eulerAngles = new Vector3(0, 0, (-deltaX * rotationSpeed));
                }
                else if (deltaX < 0)
                {
                    //Debug.Log("tilt left");
                    cardRectTransform.eulerAngles = new Vector3(0, 0, (deltaX * -rotationSpeed));
                }
                else
                {
                    //Debug.Log("no tilt");
                    cardRectTransform.eulerAngles = new Vector3(0, 0, 0);
                }
            }

        }

    }


}