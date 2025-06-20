using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TutorialController : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    public GameObject blockerPanel;
    public List<TutorialStep> steps;

    private void Start()
    {
        steps = new List<TutorialStep>
        {
            new TutorialStep
            {
                message = "First, draw four cards from the Number Deck. And I will do the same.",
                requireContinue = true,
                afterContinue = () =>
                {
                    StartCoroutine(TutorialScripts.CardPlacementController.instance.DealNumbers());
                },
                waitUntil = () => TutorialScripts.CardPlacementController.instance.doneDealing
            },
            new TutorialStep
            {
                message = "The Number deck has number cards, they can be positive or negative. The number on the left side of the screen is your number. The number on the right side of the screen is your opponent�s number.\r\n",
                requireContinue = true,

            }


        };


        StartCoroutine(RunTutorial());
    }

    IEnumerator RunTutorial()
    {
        
        foreach (var step in steps)
        {
            blockerPanel.SetActive(true);
            tutorialText.text = step.message;

            // Wait for click if required
            if (step.requireContinue)
            {
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            }

            step.afterContinue?.Invoke();
            blockerPanel.SetActive(false);

            if (step.waitUntil != null)
            yield return new WaitUntil(step.waitUntil);
            yield return new WaitForSeconds(step.autoAdvanceDelay);

        }

        blockerPanel.SetActive(false);
    }
}
