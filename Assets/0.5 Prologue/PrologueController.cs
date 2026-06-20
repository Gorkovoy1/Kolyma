using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AK.Wwise;

public class PrologueController : MonoBehaviour
{
    public List<PrologueStepData> steps;
    public PrologueText prologueText;
    public Canvas recordCanvas;
    public GameObject dialogueBox;
    public int cardIndex = 0;
    public GameObject[] cards;
    public string soundName;
    [SerializeField] RecordSpinner recordSpinner;
    public float fadeDuration = 1f;
    public CanvasGroup dialogueCanvasGroup;

    IEnumerator FadeInDialogue()
    {
        dialogueCanvasGroup.alpha = 0f;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            dialogueCanvasGroup.alpha =
                Mathf.Lerp(0f, 1f, timer / fadeDuration);

            yield return null;
        }

        dialogueCanvasGroup.alpha = 1f;
    }

    IEnumerator FadeOutCanvas()
    {
        this.gameObject.GetComponentInChildren<CanvasGroup>().alpha = 1f;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            this.gameObject.GetComponentInChildren<CanvasGroup>().alpha =
                Mathf.Lerp(1f, 0f, timer / fadeDuration);

            yield return null;
        }

        this.gameObject.GetComponentInChildren<CanvasGroup>().alpha = 0f;

        StartCoroutine(SceneLoader.instance.LoadNextScene("3 DIALOGUE1 Jail"));
    }

    IEnumerator Start()
    {

        AkSoundEngine.PostEvent("Play_Prologue_Ambience", this.gameObject);
        yield return new WaitForSeconds(2f);
        AkSoundEngine.PostEvent("Play_Woosh_Narrator", this.gameObject);

        yield return StartCoroutine(FadeInDialogue());

        steps = new List<PrologueStepData>
        {
            new PrologueStepData
            {
                dialogue = true,
                speaker = "Narrator",
                message = "He remembered that hot summer night. His mother had opened all the windows " +
                "in the apartment to let some fresh air in.",
                requireContinue = true,
            },
            new PrologueStepData
            {
                dialogue = true,
                speaker = "Narrator",
                message = "It was late, but both of them were awake, listening " +
                "to the silence of the dark, empty streets.",
                requireContinue = true,
            },
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
                stopSounds = true,
                activateNextCard = true,

                soundName1 = "Play_Doorbell1",
                soundName2 = "Play_Knock1",

                dialogue = true,
                speaker = "Male Voice",
                message = "Open up! We need to speak to Arkady Kojukh!",
                requireContinue = true,
            },
            new PrologueStepData
            {
                speaker = "Sofia",
                message = "<i>Arkady, they have come for you!</i>",
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
                playSameTime = true,
                soundName1 = "Play_Doorbell2",
                soundName2 = "Play_Knock2",
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
                message = "<i>Stay in your room. I will talk to them. Maybe they will listen.</i>",
                requireContinue = true,
            },
            new PrologueStepData
            {
                activateNextCard = true,
                speaker = "Male Voice",
                message = "Sofia Nikolaevna Kojukh? Sergeant Zverev.  We need to see your son Arkady.",
                requireContinue = true,
            },
            new PrologueStepData
            {
                speaker = "Sofia",
                message = "Why?",
                requireContinue = true,
            },
            new PrologueStepData
            {
                speaker = "Sergeant Zverev",
                message = "He’s been summoned for an urgent interrogation. He will have to come with us.",
                requireContinue = true,
            },
            new PrologueStepData
            {
                speaker = "Sofia",
                message = "Please do not take my son away! He is innocent! I signed all the papers, I have no contact with my husband, our marriage is nullified. Arkady is innocent!",
                requireContinue = true,
            },
            new PrologueStepData
            {
                speaker = "Sergeant Zverev",
                message = "No need to worry, if he is innocent, the investigators will release him tomorrow.",
                requireContinue = true,
            },
            new PrologueStepData
            {
                activateNextCard = true,
                speaker = "Sergeant Zverev",
                message = "Solomin and Zubov - search the place and protocol everything. Volkov - get the boy.",
                requireContinue = true,
                afterContinue = () =>
                {
                    cards[cardIndex].SetActive(true);
                    cardIndex++;
                },

            },
            new PrologueStepData
            {
                activateNextCard = true,
                soundName1 = "Play_Fight",
                speaker = "Zubov",
                message = "The boy is here! He is resisting!",
                requireContinue = true,
            },
            new PrologueStepData
            {
                speaker = "Sergeant Zverev",
                message = "Volkov, calm him down.",
                requireContinue = true,
                
            },
            new PrologueStepData
            {
                activateNextCard = true,
                speaker = "Sofia",
                message = "Arkady!",
                requireContinue = true,
            },
            new PrologueStepData
            {
                activateNextCard = true,
                soundName1 = "Play_Handcuffs",
                speaker = "Sergeant Zverev",
                message = "Arkady Kojukh, you are under arrest for organizing a terrorist group in the interests of the foreign imperialist states.",
                requireContinue = true,
            },
            new PrologueStepData
            {
                speaker = "Arkady",
                message = "Mother, I am sorry…",
                requireContinue = true,
                afterContinue = () =>
                {
                    //fade canvas out
                    StartCoroutine(FadeOutCanvas());
                },
            }
        };

        yield return StartCoroutine(RunPrologue());
    }

    void OnSoundFinished(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {
        AkSoundEngine.PostEvent(soundName, this.gameObject);
        soundName = null;
    }

    IEnumerator RunPrologue()
    {
        foreach (var step in steps)
        {
            prologueText.text = step.message;
            //prologueText.name = step.speaker;
            if(step.speaker == "Narrator")
            {
                prologueText.GetComponentInChildren<TMP_Text>(true).color = Color.white;
            }
            else if(step.speaker == "Sofia Petrovna" || step.speaker == "Sofia")
            {
                prologueText.GetComponentInChildren<TMP_Text>(true).color = Color.yellow;
            }
            else if(step.speaker == "Arkady")
            {
                prologueText.GetComponentInChildren<TMP_Text>(true).color = new Color32(14, 172, 14, 255);
            }
            else if(step.speaker == "Male Voice" || step.speaker == "Sergeant Zverev")
            {
                prologueText.GetComponentInChildren<TMP_Text>(true).color = Color.red;
            }
            else if(step.speaker == "Zubov")
            {
                prologueText.GetComponentInChildren<TMP_Text>(true).color = new Color32(0, 155, 255, 255);
            }

            prologueText.ShowLine();
            dialogueBox.SetActive(step.dialogue);

            if (step.activateNextCard)
            {
                if (cardIndex == 1)
                {
                    //play door open
                    AkSoundEngine.PostEvent(
                    "Play_Door_Opens",
                    this.gameObject,
                    (uint)AkCallbackType.AK_EndOfEvent,
                    OnSoundFinished,
                    null);

                    soundName = "Play_Cigarette";

                    //play smoke after and show card
                    yield return new WaitForSeconds(2.2f);
                }
                else if (cardIndex == 5)
                {
                    AkSoundEngine.PostEvent("Play_Rifle_Butt_Hit", this.gameObject);
                    this.gameObject.GetComponentInChildren<UIScreenShake>().Shake(0.25f, 15f);
                }
                cards[cardIndex].SetActive(true);
                cardIndex++;
            }

            if (step.stopSounds)
            {
                AkSoundEngine.PostEvent(step.soundName1, this.gameObject);
                yield return new WaitForSeconds(1.3f);
                AkSoundEngine.PostEvent(step.soundName2, this.gameObject);
                AkSoundEngine.PostEvent("Play_Music_Prologue_Arrest", this.gameObject);
                yield return new WaitForSeconds(2f);
                //AkSoundEngine.PostEvent("Play_Record_Stops", this.gameObject);
                AkSoundEngine.StopPlayingID(recordSpinner.musicId);
                
            }
            else if(step.soundName1 != null && step.soundName2 != null)
            {
                if(step.playSameTime)
                {
                    AkSoundEngine.PostEvent(step.soundName1, this.gameObject);
                    AkSoundEngine.PostEvent(step.soundName2, this.gameObject);
                }
                else
                {
                    AkSoundEngine.PostEvent(
                    step.soundName1,
                    this.gameObject,
                    (uint)AkCallbackType.AK_EndOfEvent,
                    OnSoundFinished,
                    null);

                    soundName = step.soundName2;
                }
                
            }
            else if(step.soundName1 != null && step.soundName2 == null)
            {
                AkSoundEngine.PostEvent(step.soundName1, this.gameObject);
            }

            


            if (step.requireContinue)
            {
                Debug.Log("Waiting for click...");
                yield return new WaitUntil(() => !prologueText.isTyping && Input.GetMouseButtonDown(0));
                Debug.Log("Clicked!");
            }

            step.afterContinue?.Invoke();


            if (step.waitUntil != null)
            {
                yield return new WaitUntil(step.waitUntil);
            }

            yield return new WaitForSeconds(step.autoAdvanceDelay);
        }

        //blockerPanel.SetActive(false);
    }
}
