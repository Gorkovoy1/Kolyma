using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;
using AK.Wwise;

public class CardPlacementController : MonoBehaviour
{
    public GameObject twoSpace;
    public GameObject threeSpace;
    public GameObject fourSpace;
    public GameObject fiveSpace;
    public GameObject sixSpace;
    public GameObject sevenSpace;
    public GameObject eightSpace;
    public GameObject nineSpace;
    public GameObject negTwoSpace;
    public GameObject negThreeSpace;
    public GameObject negFourSpace;
    public GameObject negFiveSpace;

    public Transform opponentPositiveArea;
    public Transform opponentNegativeArea;
    public Transform playerPositiveArea;
    public Transform playerNegativeArea;

    public Transform numberDeckTransform;

    public List<GameObject> numberDeck;

    public static CardPlacementController instance;

    public Canvas battleCanvas;
    public Camera mainCamera;
    public Camera diceCamera;

    public GameObject sfxObj;

    public bool fadingIn = false;
    public bool fadingOut = false;

    public Image blackImage;

    public float elapsed = 0f;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DealNumbers());
    }

    // Update is called once per frame
    void Update()
    {
        if (fadingIn)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / 1f);
            Color color = blackImage.color;
            color.a = alpha;
            blackImage.color = color;

            if (alpha >= 1f)
            {
                fadingIn = false; // Fade finished
            }
        }
        else if (fadingOut)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (elapsed / 1f));
            Color color = blackImage.color;
            color.a = alpha;
            blackImage.color = color;

            if (alpha <= 0f)
            {
                fadingOut = false; // Fade finished
            }
        }
    }

    IEnumerator DealNumbers()
    {
        ShuffleNumbers();
        yield return StartCoroutine(DealPlayerNumbers());  
        yield return StartCoroutine(DealOpponentNumbers()); 

    }

    IEnumerator DealPlayerNumbers()
    {
        for (int i = 0; i < 4; i++)
        {
            if (numberDeck.Count == 0)
            {
                Debug.LogWarning("numberDeck is empty, stopping coroutine.");
                yield break; // Stop the coroutine if there are no more numbers
            }

            //give each number appropriate tag, then search for tags in scene for conditionals
            //or check each gameobj for stats

            //instantiate the space and then the image
            //the image is the one that is clickable
            //on start instantiate space to parent opponent or player, show image, then move to space
            //will know which by parent 
            //deck of spaces
            if (numberDeck[0].GetComponent<NumberStats>().positive)
            {
                Instantiate(numberDeck[0], playerPositiveArea);
                numberDeck.RemoveAt(0);
            }
            else if(numberDeck[0].GetComponent<NumberStats>().negative)
            {
                Instantiate(numberDeck[0], playerNegativeArea);
                numberDeck.RemoveAt(0);
            }

            //Delay between cards
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator DealOpponentNumbers()
    {
        for (int i = 0; i < 4; i++)
        {
            if (numberDeck.Count == 0)
            {
                Debug.LogWarning("numberDeck is empty, stopping coroutine.");
                yield break; // Stop the coroutine if there are no more numbers
            }

            //give each number appropriate tag, then search for tags in scene for conditionals
            //or check each gameobj for stats

            //instantiate the space and then the image
            //the image is the one that is clickable
            //on start instantiate space to parent opponent or player, show image, then move to space
            //will know which by parent 
            //deck of spaces
            if (numberDeck[0].GetComponent<NumberStats>().positive)
            {
                Instantiate(numberDeck[0], opponentPositiveArea);
                numberDeck.RemoveAt(0);
            }
            else if (numberDeck[0].GetComponent<NumberStats>().negative)
            {
                Instantiate(numberDeck[0], opponentNegativeArea);
                numberDeck.RemoveAt(0);
            }

            //Delay between cards
            yield return new WaitForSeconds(1f);
        }

        fadingIn = true;
        StartCoroutine(RollDice());

        
    }

    IEnumerator RollDice()
    {
        yield return new WaitForSeconds(0.5f);
        AkSoundEngine.PostEvent("Play_Dice", sfxObj);
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene("DiceRoll", LoadSceneMode.Additive);
        yield return null;
        diceCamera = GameObject.FindGameObjectWithTag("DiceCamera").GetComponent<Camera>();
        mainCamera.gameObject.SetActive(false);
        battleCanvas.enabled = false;
        diceCamera.gameObject.SetActive(true);
        yield return new WaitForSeconds(4.3f);
        //unload scene
        mainCamera.gameObject.SetActive(true);
        diceCamera.gameObject.SetActive(false);
        battleCanvas.enabled = true;
        SceneManager.UnloadSceneAsync("DiceRoll");
        elapsed = 0f;
        fadingOut = true;
        Debug.Log("fade out");
        yield return new WaitForSeconds(1f);
        Debug.Log("done");
        
        
        //update target value with player prefs
        Debug.Log("Target is: " + PlayerPrefs.GetInt("TargetValue", 0));
        
        NumberManager.instance.targetVal = PlayerPrefs.GetInt("TargetValue", 0);

        //if value is higher
        TurnManager.instance.isPlayerTurn = true;
    }

    public void DealOneCard(string target)
    {

        if(target == "player")
        {
            if (numberDeck.Count != 0)
            {
                if (numberDeck[0].GetComponent<NumberStats>().positive)
                {
                    Instantiate(numberDeck[0], playerPositiveArea);
                    numberDeck.RemoveAt(0);
                }
                else if (numberDeck[0].GetComponent<NumberStats>().negative)
                {
                    Instantiate(numberDeck[0], playerNegativeArea);
                    numberDeck.RemoveAt(0);
                }
            }
        }
        else if(target == "opponent")
        {
            if (numberDeck.Count != 0)
            {
                if (numberDeck[0].GetComponent<NumberStats>().positive)
                {
                    Instantiate(numberDeck[0], opponentPositiveArea);
                    numberDeck.RemoveAt(0);
                }
                else if (numberDeck[0].GetComponent<NumberStats>().negative)
                {
                    Instantiate(numberDeck[0], opponentNegativeArea);
                    numberDeck.RemoveAt(0);
                }
            }
        }
        
        

    }


    void ShuffleNumbers()
    {
        for (int i = numberDeck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            GameObject temp = numberDeck[i];
            numberDeck[i] = numberDeck[j];
            numberDeck[j] = temp;
        }
    }
}
