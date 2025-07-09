using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TutorialScripts
{
    public class ChoiceController : MonoBehaviour
    {

        public GameObject choiceObject;
        public GameObject discardChoiceObject;

        public GameObject buttonPrefab;
        public GameObject buttonPrefabCard;

        public Sprite negFour;
        public Sprite posFour;
        public Sprite negTwo;
        public Sprite posTwo;

        public static ChoiceController instance;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ShowChoice(int x)
        {
            //delete all children
            foreach (Transform child in choiceObject.transform)
            {
                Destroy(child.gameObject);
            }
            //show correct buttons
            if (x == 2)
            {


                ///////////////////////////
                GameObject buttonObjTwo = Instantiate(buttonPrefab, choiceObject.transform);
                Button buttonTwo = buttonObjTwo.GetComponent<Button>();
                Image buttonTwoImage = buttonTwo.GetComponent<Image>();

                buttonTwoImage.sprite = posTwo;


                buttonTwo.onClick.AddListener(() =>
                {
                    SpecialCardManager.instance.Give(2, "opponent");
                    choiceObject.SetActive(false);
                });

                /////////////////////////////
                ///
                GameObject buttonObjOne = Instantiate(buttonPrefab, choiceObject.transform);
                Button buttonOne = buttonObjOne.GetComponent<Button>();
                Image buttonOneImage = buttonOne.GetComponent<Image>();

                buttonOneImage.sprite = negTwo;


                buttonOne.onClick.AddListener(() =>
                {
                    //play error noise
                    Debug.Log("give 2!");
                });
            }
            else if (x == 4)
            {

                ///////////////////////////
                GameObject buttonObjTwo = Instantiate(buttonPrefab, choiceObject.transform);
                Button buttonTwo = buttonObjTwo.GetComponent<Button>();
                Image buttonTwoImage = buttonTwo.GetComponent<Image>();

                buttonTwoImage.sprite = posFour;


                buttonTwo.onClick.AddListener(() =>
                {
                    SpecialCardManager.instance.Give(4, "opponent");
                    choiceObject.SetActive(false);
                });
                ////////////////////////////
                ///
                GameObject buttonObjOne = Instantiate(buttonPrefab, choiceObject.transform);
                Button buttonOne = buttonObjOne.GetComponent<Button>();
                Image buttonOneImage = buttonOne.GetComponent<Image>();

                buttonOneImage.sprite = negFour;


                buttonOne.onClick.AddListener(() =>
                {
                    SpecialCardManager.instance.Give(-4, "opponent");
                    choiceObject.SetActive(false);
                });

            }




            choiceObject.SetActive(true);
        }

        public void ShowDiscardedCards(List<GameObject> discardedCards, Transform playerHand)
        {
            foreach (Transform child in discardChoiceObject.transform)
            {
                Destroy(child.gameObject);
            }


            foreach (GameObject card in discardedCards)
            {
                GameObject chosenCard = card;
                GameObject buttonObjTwo = Instantiate(buttonPrefabCard, discardChoiceObject.transform);
                Button buttonTwo = buttonObjTwo.GetComponent<Button>();
                Image buttonTwoImage = buttonObjTwo.GetComponent<Image>();
                buttonTwoImage.sprite = chosenCard.GetComponent<CardPlace>().imagePrefab.transform.Find("Image").GetComponent<Image>().sprite;
                TextMeshProUGUI buttonText = buttonTwo.GetComponentInChildren<TextMeshProUGUI>();
                Transform[] allChildren = chosenCard.GetComponent<CardPlace>().imagePrefab.GetComponentsInChildren<Transform>(true);
                foreach (Transform child in allChildren)
                {
                    if (child.name == "Text (TMP) (1)")
                    {
                        buttonText.text = child.GetComponent<TextMeshProUGUI>().text;
                        buttonText.color = Color.white;
                        break;
                    }
                }


                buttonTwo.onClick.AddListener(() =>
                {
                    discardedCards.Remove(card);
                    chosenCard.GetComponent<CardPlace>().beingPlayed = false;
                    chosenCard.GetComponent<CardPlace>().correspondingImage.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    chosenCard.transform.SetParent(playerHand);
                    discardChoiceObject.SetActive(false);
                });

            }



            discardChoiceObject.SetActive(true);


        }
    }


}