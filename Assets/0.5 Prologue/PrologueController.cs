using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrologueController : MonoBehaviour
{
    public List<PrologueStepData> steps;
    public PrologueText prologueText;
    public GameObject creditsObj;

    private async void Start()
    {
        steps = new List<PrologueStepData>
        {
            new PrologueStepData
            {
                speaker = "Sofia Petrovna",
                message = "Arkady, could you please put on some music?",
                requireContinue = true,
                /*
                afterContinue = () =>
                {

                },
                waitUntil = () => TutorialScripts.CardPlacementController.instance.doneDealing,
                */
            },
            new PrologueStepData
            {
                speaker = "Arkady",
                message = "Of course, Mother.",
                requireContinue = true,

                afterContinue = () =>
                {
                    //deactivate dialogue obj

                    //activate draggable record

                    //activate recordplayer
                    
                    //in record player script, once record dragged, activate and start scrolling credits
                },
                
            },
            new PrologueStepData
            {
                
            }
        };

        StartCoroutine(RunPrologue());
    }

    IEnumerator RunPrologue()
    {
        foreach (var step in steps)
        {
            prologueText.text = step.message;
            prologueText.name = step.speaker;
            prologueText.ShowLine();
            


            if (step.requireContinue)
            {
                Debug.Log("Waiting for click...");
                yield return new WaitUntil(() => !prologueText.isTyping && Input.GetMouseButtonDown(0));
                Debug.Log("Clicked!");
            }

            step.afterContinue?.Invoke();

            //blockerPanel.SetActive(false);

            if (step.waitUntil != null)
            {
                yield return new WaitUntil(step.waitUntil);
            }

            yield return new WaitForSeconds(step.autoAdvanceDelay);
        }

        //blockerPanel.SetActive(false);
    }
}
