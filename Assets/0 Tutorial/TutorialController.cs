using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TutorialScripts;


public class TutorialController : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    public GameObject blockerPanel;
    public List<TutorialStepData> steps;

    public GameObject opponentHand;
    public GameObject playerHand;

    public GameObject playerDiscard;

    public Canvas DeckManagerCanvas;

    public GameObject tutorialObj;

    private async void Start()
    {
        tutorialObj = this.gameObject;

        steps = new List<TutorialStepData>
        {
            new TutorialStepData
            {
                message = "First, draw four cards from the Number Deck. And I will do the same.",
                requireContinue = true,
                afterContinue = () =>
                {
                    StartCoroutine(TutorialScripts.CardPlacementController.instance.DealNumbers());
                },
                waitUntil = () => TutorialScripts.CardPlacementController.instance.doneDealing,

            },
            new TutorialStepData
            {
                message = "The Number deck has number cards, they can be positive or negative. The number on the left side of the screen is your number. The number on the right side of the screen is your opponent’s number.",
                requireContinue = true,

            },
            new TutorialStepData
            {
                message = "Let’s roll the dice. The sum of the four dice will set the target.",
                requireContinue = true,

            },
            new TutorialStepData
            {
                message = "You have to make your number reach the target or get as close as possible.",
                requireContinue = true,

            },
            new TutorialStepData
            {
                message = "But be careful! If you go busted, you will lose the game.",
                requireContinue = true,
                afterContinue = () =>
                {
                    //roll the dice
                },
                //waitUntil = () => doneRolling
            },
            new TutorialStepData
            {
                message = "The number in the middle is called The Target. Your goal is to manipulate the cards so your number gets as close as possible to the target, while your opponent should be either too high or too low. \r\nThe player whose number exceeds the Target loses the game.",
                requireContinue = true,
            },
            new TutorialStepData
            {
                message = "Now it’s time to select your tricks.",
                requireContinue = true,
            },
            new TutorialStepData
            {
                message = "Tricks are the cards that allow you to perform actions and change the numbers on the table.",
                requireContinue = true,
            },
            new TutorialStepData
            {
                message = "Each player selects 15 tricks from their decks. They will use these 15 cards for the rest of the current Game of Numbers, so you have to choose wisely.",
                requireContinue = true,
            },
            new TutorialStepData
            {
                message = "Your deck will get bigger, but right now you only have 6 tricks, so go ahead and select all of them.",
                requireContinue = true,
                afterContinue = () =>
                {
                    //activate deck manager
                    DeckManagerCanvas.GetComponentInChildren<DeckManagerController>().ShowCanvas();
                },
                //wait until finish deck button clicked
                //then deactivate canvas and go to next step
                waitUntil = () => DeckManagerCanvas.GetComponent<DeckManagerController>().finishDeck,
            },
            new TutorialStepData
            {
                message = "We start with 6 cards in your hand at the beginning of every game. Let's draw those.",
                requireContinue = true,
                afterContinue = () =>
                {
                    //draw cards
                    opponentHand.GetComponentInParent<TutorialScripts.HandController>().ShuffleSpecials();
                },
                waitUntil = () => opponentHand.GetComponentInParent<TutorialScripts.HandController>().doneDealing,

            },
            new TutorialStepData
            {
                message = "My dice number is bigger, so I make the first move.",
                requireContinue = true,
                afterContinue = () =>
                {
                    //play Andreyev's turn
                    tutorialObj.GetComponent<TutorialScripts.AIController>().selectedCardToPlay = opponentHand.transform.GetChild(0).gameObject;
                    tutorialObj.GetComponent<TutorialScripts.AIController>().PlayCard();
                    
                },
                //wait until card has been played and 2 is swapped for 8
                waitUntil = () => NumberManager.instance.playerVal == 14,
            },
            new TutorialStepData
            {
                message = "Now it's your turn. Select Thick Woolen Coat and give your opponent +2.",
                requireContinue = true,
                afterContinue = () =>
                {
                    TurnManager.instance.isPlayerTurn = true;
                    //set everything as unplayable
                    foreach(Transform child in playerHand.transform)
                    {
                        if(child.gameObject.name == "ThickWoolenCoat(Clone)")
                        {
                            child.gameObject.GetComponent<TutorialScripts.CardPlace>().isPlayable = true;
                        }
                        else
                        {
                            child.gameObject.GetComponent<TutorialScripts.CardPlace>().isPlayable = false;
                        }
                    }
                    //set thick woolen coat as playable
                    
                    

                },
                //wait until thick woolen coat is played
                waitUntil = () => NumberManager.instance.oppVal == 11,
            },
            new TutorialStepData
            {
                message = "I must say, you are a fast learner. Here’s another lesson. It’s always better when your opponent has fewer tricks than you.",
                requireContinue = true,
                afterContinue = () =>
                {
                    TurnManager.instance.isPlayerTurn = false;
                    tutorialObj.GetComponent<TutorialScripts.AIController>().selectedCardToPlay = opponentHand.transform.GetChild(0).gameObject;
                    tutorialObj.GetComponent<TutorialScripts.AIController>().PlayCard();

                },
                //wait until card is played (make sure ot discard empty pockets)
                //wait until empty pockets is in discard pile
                waitUntil = () => playerDiscard.transform.childCount == 2,
            },
            new TutorialStepData
            {
                message = "Use one of your own tricks to respond. Play Setup and make him get rid of his -4.",
                requireContinue = true,
                afterContinue = () =>
                {
                    TurnManager.instance.isPlayerTurn = true;
                    foreach(Transform child in playerHand.transform)
                    {
                        if(child.gameObject.name == "Setup(Clone)")
                        {
                            child.gameObject.GetComponent<TutorialScripts.CardPlace>().isPlayable = true;
                        }
                        else
                        {
                            child.gameObject.GetComponent<TutorialScripts.CardPlace>().isPlayable = false;
                        }
                    }
                },
                //wait until one less number
            }




        };


        StartCoroutine(RunTutorial());
    }

    IEnumerator RunTutorial()
    {
        foreach (var step in steps)
        {
            Debug.Log("Tutorial step: " + step.message);
            blockerPanel.SetActive(true);
            tutorialText.text = step.message;

            if (step.requireContinue)
            {
                Debug.Log("Waiting for click...");
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
                Debug.Log("Clicked!");
            }

            step.afterContinue?.Invoke();

            blockerPanel.SetActive(false);

            if (step.waitUntil != null)
            {
                yield return new WaitUntil(step.waitUntil);
            }

            yield return new WaitForSeconds(step.autoAdvanceDelay);
        }

        blockerPanel.SetActive(false);
    }
}
