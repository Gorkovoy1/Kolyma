using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TutorialScripts;
using UnityEngine.SceneManagement;


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

    public GameObject weaknessCard;

    public GameObject sfxObj;

    public Button flipButton;

    public Camera mainCamera;
    public Camera diceCamera;
    public Canvas tutorialCanvas;

    public bool doneRolling = false;

    public BattleEndController battleEndController;

    IEnumerator SwitchToDiceScene()
    {
        SceneManager.LoadScene("DiceRoll", LoadSceneMode.Additive);
        yield return null;
        diceCamera = GameObject.FindGameObjectWithTag("DiceCamera").GetComponent<Camera>();
        mainCamera.gameObject.SetActive(false);
        tutorialCanvas.enabled = false;
        diceCamera.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        doneRolling = true;
        mainCamera.gameObject.SetActive(true);
        diceCamera.gameObject.SetActive(false);
        tutorialCanvas.enabled = true;


        //update target value with player prefs
        Debug.Log("Target is: " + PlayerPrefs.GetInt("TargetValue", 0));
        NumberManager.instance.targetVal = 16;
        //NumberManager.instance.targetVal = PlayerPrefs.GetInt("TargetValue", 0);
        //above line is for non tutorial scene

        //unload scene
        SceneManager.UnloadSceneAsync("DiceRoll");
    }

    private async void Start()
    {
        tutorialObj = this.gameObject;
        mainCamera = FindObjectOfType<Camera>();
        

        steps = new List<TutorialStepData>
        {
            new TutorialStepData
            {
                setPosition = new Vector2(242, -167), //484, 333
                message = "First, draw four cards from the Number Deck. I will do the same.",
                requireContinue = true,
                afterContinue = () =>
                {

                    StartCoroutine(TutorialScripts.CardPlacementController.instance.DealNumbers());
                },
                waitUntil = () => TutorialScripts.CardPlacementController.instance.doneDealing,

            },
            new TutorialStepData
            {
                setPosition = new Vector2(242, -167),
                message = "The Number deck has positives and negatives. The value at the top is mine. The bottom one is yours.",
                requireContinue = true,

            },
            new TutorialStepData
            {
                message = "Let’s roll the dice. The sum of the four dice will determine the target.",
                requireContinue = true,

            },
            new TutorialStepData
            {
                message = "You must get as close to the target value as possible.",
                requireContinue = true,

            },
            new TutorialStepData
            {
                message = "But be careful! If you are over at the end, you lose.",
                requireContinue = true,
                afterContinue = () =>
                {
                    StartCoroutine(SwitchToDiceScene());
                    

                },
                waitUntil = () => doneRolling,
                //when done rolling bool, then switch cameras back 
            },
            new TutorialStepData
            {
                message = "The value in the top left is the Target. Your goal is to manipulate the cards so that you are closer to the Target than your opponent.",
                requireContinue = true,
            },
            new TutorialStepData
            {
                message = "Now it’s time to select your tricks.",
                requireContinue = true,
            },
            new TutorialStepData
            {
                message = "Tricks are cards that allow you to change the numbers on the table.",
                requireContinue = true,
            },
            new TutorialStepData
            {
                message = "To start, each player selects 15 tricks from their deck. These are the tricks you will use for the Game of Numbers, so choose wisely.",
                requireContinue = true,
            },
            new TutorialStepData
            {
                message = "Your deck will get bigger, but right now you only have 6 tricks. Go ahead and select all of them.",
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
                message = "We always start with 6 cards in hand. Let's draw those.",
                requireContinue = true,
                afterContinue = () =>
                {
                    //draw cards
                    TurnManager.instance.isPlayerTurn = true;
                    opponentHand.GetComponentInParent<TutorialScripts.HandController>().ShuffleSpecials();
                },
                waitUntil = () => opponentHand.GetComponentInParent<TutorialScripts.HandController>().doneDealing,

            },
            new TutorialStepData
            {
                message = "I rolled higher, so I go first.",
                requireContinue = true,
                afterContinue = () =>
                {
                    //play Andreyev's turn
                    TurnManager.instance.isPlayerTurn = false;
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
                            weaknessCard = child.gameObject;
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
                waitUntil = () => NumberManager.instance.oppVal == 13,
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
                message = "Respond with one of your own tricks. Play Setup and make him get rid of his -4.",
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
                //wait until andreyev only ahs 1 negative
                waitUntil = () => NumberManager.instance.OPPnegatives.Count == 1,
            },
            new TutorialStepData
            {
                message = "One of your numbers is red, which allows me to use this trick.",
                requireContinue = true,
                afterContinue = () =>
                {
                    TurnManager.instance.isPlayerTurn = false;
                    //play Andreyev's turn
                    tutorialObj.GetComponent<TutorialScripts.AIController>().selectedCardToPlay = opponentHand.transform.GetChild(0).gameObject;
                    tutorialObj.GetComponent<TutorialScripts.AIController>().PlayCard();
                },
                //wait until one more number
                waitUntil = () => NumberManager.instance.positives.Count == 4,
            },
            new TutorialStepData
            {
                message = "You gave Andreyev a yellow card earlier. Time to use it to your advantage. Play Weakness.",
                requireContinue = true,
                afterContinue = () =>
                {
                    TurnManager.instance.isPlayerTurn = true;
                    foreach(Transform child in playerHand.transform)
                    {
                        if(child.gameObject.name == "Weakness(Clone)")
                        {

                            child.gameObject.GetComponent<TutorialScripts.CardPlace>().isPlayable = true;
                        }
                        else
                        {
                            child.gameObject.GetComponent<TutorialScripts.CardPlace>().isPlayable = false;
                        }
                    }
                },
                //wait until one less card in discard pile
                waitUntil = () => weaknessCard.transform.parent.transform == playerHand.transform,
            },
            new TutorialStepData
            {
                message = "Let's see if you can solve this problem.",
                requireContinue = true,
                afterContinue = () =>
                {
                    TurnManager.instance.isPlayerTurn = false;
                    tutorialObj.GetComponent<TutorialScripts.AIController>().selectedCardToPlay = opponentHand.transform.GetChild(0).gameObject;
                    tutorialObj.GetComponent<TutorialScripts.AIController>().PlayCard();
                },
                //wait until one less negative card
                waitUntil = () => NumberManager.instance.negatives.Count == 0,
            },
            new TutorialStepData
            {
                message = "Time to give your opponent another 2.",
                requireContinue = true,
                afterContinue = () =>
                {
                    TurnManager.instance.isPlayerTurn = true;
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
                },
                //wait until one more pos
                waitUntil = () => NumberManager.instance.OPPpositives.Count == 4,
            },
            new TutorialStepData
            {
                message = "Too many 2's on my board, I think I can give one back to you.",
                requireContinue = true,
                afterContinue = () =>
                {
                    TurnManager.instance.isPlayerTurn = false;
                    tutorialObj.GetComponent<TutorialScripts.AIController>().selectedCardToPlay = opponentHand.transform.GetChild(0).gameObject;
                    tutorialObj.GetComponent<TutorialScripts.AIController>().PlayCard();
                },
                //wait until one less negative card
                waitUntil = () => NumberManager.instance.positives.Count == 5,
            },
            new TutorialStepData
            {
                message = "Your opponent has one trick left. Make sure he cannot use it - play Poison.",
                requireContinue = true,
                afterContinue = () =>
                {
                    TurnManager.instance.isPlayerTurn = true;
                    foreach(Transform child in playerHand.transform)
                    {
                        if(child.gameObject.name == "Poison(Clone)")
                        {
                            child.gameObject.GetComponent<TutorialScripts.CardPlace>().isPlayable = true;
                        }
                        else
                        {
                            child.gameObject.GetComponent<TutorialScripts.CardPlace>().isPlayable = false;
                        }
                    }
                },
                //wait until one more pos
                waitUntil = () => opponentHand.transform.childCount == 0,
            },
            new TutorialStepData
            {
                message = "Good! I'm out of tricks. However, I can still flip or swap any of my Numbers.",
                requireContinue = true,
                afterContinue = () =>
                {
                    TurnManager.instance.isPlayerTurn = false;
                    //flip 2 for opponent
                    foreach(GameObject g in NumberManager.instance.OPPpositives)
                    {
                        if(g.GetComponent<NumberStats>().value == 2)
                        {
                            StartCoroutine(TutorialScripts.CardSelectionController.instance.FlipNumber(g));
                            break;
                        }
                    }
                },
                //wait until flip
                waitUntil = () => NumberManager.instance.OPPnegatives.Count == 2,

            },
            new TutorialStepData
            {
                message = "Each player has one action they can use during the Game of Numbers. They can either flip or swap out any of their Numbers. Andreyev chose to flip his 2.",
                requireContinue = true,

            },
            new TutorialStepData
            {
                message = "Use your last trick. May the numbers be on your side.",
                requireContinue = true,
                afterContinue = () =>
                {
                    TurnManager.instance.isPlayerTurn = true;
                    foreach(Transform child in playerHand.transform)
                    {
                        if(child.gameObject.name == "CaughtRedHanded(Clone)")
                        {
                            child.gameObject.GetComponent<TutorialScripts.CardPlace>().isPlayable = true;
                        }
                        else
                        {
                            child.gameObject.GetComponent<TutorialScripts.CardPlace>().isPlayable = false;
                        }
                    }
                },
                //wait until one more pos
                waitUntil = () => NumberManager.instance.oppVal == 24,
            },
            new TutorialStepData
            {
                message = "Now, use your action. Flip your 4 to secure the victory.",
                requireContinue = true,
                afterContinue = () =>
                {
                    TurnManager.instance.isPlayerTurn = true;
                    flipButton.GetComponent<Button>().enabled = true;
                },
                //
                waitUntil = () => NumberManager.instance.playerAction
            },
            new TutorialStepData
            {
                message = "Good work. Since we are out of cards, we both pass. Two turns passed means Game Over. Looks like the newbie wins. You wont be as lucky with the others.",
                requireContinue = true,
                afterContinue = () =>
                {
                    battleEndController.StartFade();
                }
            }

        };


        StartCoroutine(RunTutorial());
    }

    void Update()
    {
        
        if(TurnManager.instance.isPlayerTurn)
        {
            playerHand.GetComponent<HandFanController>().fanHand = true;
        }
        else //if any cards are "being played"
        {
            playerHand.GetComponent<HandFanController>().fanHand = false;
        }
    }

    IEnumerator RunTutorial()
    {
        foreach (var step in steps)
        {
            Debug.Log("Tutorial step: " + step.message);
            blockerPanel.SetActive(true);
            RectTransform parentRect = tutorialText.transform.parent as RectTransform;
            parentRect.anchoredPosition = step.setPosition;
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
