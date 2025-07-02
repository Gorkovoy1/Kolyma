using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TutorialController : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    public GameObject blockerPanel;
    public List<TutorialStepData> steps;

    public Canvas DeckManagerCanvas;

    private void Start()
    {
        DeckManagerCanvas.gameObject.SetActive(false);
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
                    DeckManagerCanvas.gameObject.SetActive(true);
                },
                //wait until finish deck button clicked
                //then deactivate canvas and go to next step
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
