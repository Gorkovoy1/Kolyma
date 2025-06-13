using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectionController : MonoBehaviour
{
    public bool selectable;
    public bool selected;

    public GameObject buttonPrefab;

    public GameObject choiceObj;

    public static CardSelectionController instance;

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
        //loop through all cards in numbermanager.instance
        //if marked selectable in number stats component, copy and make a button for it with same position
        //call the right function (swap, flip, remove, etc)
        //on select, feed it through the right function and perform the action
        //destroy all buttons
    }

    void Update()
    {
        
    }

    public void CallButtons()
    {
        foreach (Transform child in choiceObj.transform)
        {
            Destroy(child.gameObject);
        }

        choiceObj.SetActive(true);

        List<GameObject> combined = new List<GameObject>();
        combined.AddRange(NumberManager.instance.allNumbers);
        combined.AddRange(NumberManager.instance.OPPallNumbers);

        foreach (GameObject g in combined) //NOTE THAT MUST CALL BOTH PLAYER COLORS AND OPP COLORS
        {
            if(g.GetComponent<NumberStats>().selectable)
            {
                //instantiate a button, make the image the same, make the size and position the same
                GameObject buttonTemp = Instantiate(buttonPrefab, choiceObj.gameObject.transform);

                Button buttonObj = buttonTemp.GetComponent<Button>();
                Image buttonImage = buttonTemp.GetComponent<Image>();

                
                //set sprite
                Texture2D tex = g.GetComponent<CardPlace>().correspondingImage.GetComponent<RawImage>().texture as Texture2D;
                if (tex != null)
                {
                    // Create sprite the size of the texture
                    Sprite sprite = Sprite.Create(
                        tex,
                        new Rect(0, 0, tex.width, tex.height),
                        new Vector2(0.5f, 0.5f) // center pivot
                    );

                    // Set the sprite on the Button's image
                    buttonImage.sprite = sprite;

                    // Make sure it stretches to fit the button
                    buttonImage.preserveAspect = false;
                    

                    // Then scale it back to match the button's layout if needed
                    buttonImage.rectTransform.sizeDelta = buttonTemp.GetComponent<RectTransform>().sizeDelta;
                }

                buttonTemp.transform.position = g.transform.position;

                buttonObj.onClick.AddListener(() =>
                {
                    //the action that happens
                    Debug.Log("execute action");
                    choiceObj.SetActive(false);
                });

            }
        }
    }


}
