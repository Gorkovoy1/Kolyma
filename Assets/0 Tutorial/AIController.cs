using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    namespace TutorialScripts
    {
        public class AIController : MonoBehaviour
        {

            public GameObject selectedCardToPlay;

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            public void PlayCard()
            {
                selectedCardToPlay.GetComponent<TutorialScripts.AICardPlace>().AnimateBeingPlayed();
            }
        }
    }

