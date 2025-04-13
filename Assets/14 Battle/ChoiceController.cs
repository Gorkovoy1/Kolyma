using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceController : MonoBehaviour
{

    public GameObject choiceObject;

    public GameObject buttonPrefab;

    public Sprite negFour;
    public Sprite posFour;
    public Sprite negTwo;
    public Sprite posTwo;

    public static ChoiceController instance;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowChoice(int x)
    {
        if(x == 2)
        {
            GameObject buttonObjOne = Instantiate(buttonPrefab, choiceObject.transform);
            Button buttonOne = buttonObjOne.GetComponent<Button>();
            Image buttonOneImage = buttonOne.GetComponent<Image>();

            buttonOneImage.sprite = negTwo;


            buttonOne.onClick.AddListener(() =>
            {
                SpecialCardManager.instance.Give(-2, "opponent");
                choiceObject.SetActive(false);
            });

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
        }
        else if(x == 4)
        {
            GameObject buttonObjOne = Instantiate(buttonPrefab, choiceObject.transform);
            Button buttonOne = buttonObjOne.GetComponent<Button>();
            Image buttonOneImage = buttonOne.GetComponent<Image>();

            buttonOneImage.sprite = negFour;


            buttonOne.onClick.AddListener(() =>
            {
                SpecialCardManager.instance.Give(-4, "opponent");
                choiceObject.SetActive(false);
            });

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
        }
        


        choiceObject.SetActive(true);
    }
}

