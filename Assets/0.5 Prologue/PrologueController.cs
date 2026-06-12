using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrologueController : MonoBehaviour
{
    public List<PrologueStepData> steps;
    public PrologueText prologueText;
    public Canvas recordCanvas;
    public GameObject dialogueBox;

    private async void Start()
    {

        steps = new List<PrologueStepData>
        {
            new PrologueStepData
            {
                dialogue = true,
                speaker = "Sofia Petrovna",
                message = "Arkady, could you please put on some music?",
                requireContinue = true,
            },
            new PrologueStepData
            {
                dialogue = true,
                speaker = "Arkady",
                message = "Of course, Mother.",
                requireContinue = true,

                afterContinue = () =>
                {
                    //deactivate dialogue obj
                    dialogueBox.SetActive(false);
                    //activate draggable record
                    recordCanvas.gameObject.SetActive(true);
                },
                waitUntil = () => recordCanvas.GetComponentInChildren<CreditsTextController>(true).creditsEnd,

            },
            new PrologueStepData
            {
                //stop music
                //play car sound

                //animate card SEARCH

                dialogue = true,
                speaker = "Male Voice",
                message = "Open up! We need to speak to Arkady Kojukh!",
                requireContinue = true,
            },
            new PrologueStepData
            {
                speaker = "Sofia",
                message = "Arkady, wake up! They have come for you!",
                requireContinue = true,
            },
            new PrologueStepData
            {
                speaker = "Sofia",
                message = "Who is this? It’s one in the morning, what is going on?",
                requireContinue = true,
            },
            new PrologueStepData
            {
                speaker = "Male Voice",
                message = "In the name of the Soviet Government, open the door, or we will break it down.",
                requireContinue = true,
            },
            new PrologueStepData
            {
                speaker = "Sofia",
                message = "Don’t break the door! I am coming!",
                requireContinue = true,
            },
            new PrologueStepData
            {
                speaker = "Sofia",
                message = "Stay in your room. I will talk to them. Maybe they will listen.",
                requireContinue = true,
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
            dialogueBox.SetActive(step.dialogue);


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
