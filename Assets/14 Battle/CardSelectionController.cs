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

    public void CallButtons(string toDo, string target, int x = 0)
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
                    if(toDo == "swap")
                    {
                        if (g.transform.parent == CardPlacementController.instance.playerPositiveArea || g.transform.parent == CardPlacementController.instance.playerNegativeArea)
                        {
                            target = "player";
                            PlayerStats.instance.swapped = true;
                        }
                        else
                        {
                            target = "opponent";
                            OpponentStats.instance.swapped = true;
                        }

                        StartCoroutine(SwapOut(g, target));
                    }
                    else if(toDo == "discard")
                    {
                        StartCoroutine(DiscardNumber(g));
                    }
                    else if (toDo == "duplicate")
                    {
                        if(target == "opponent")
                        {
                            if (g.GetComponent<NumberStats>().positive)
                            {
                                Instantiate(g, CardPlacementController.instance.opponentPositiveArea);
                            }
                            else
                            {
                                Instantiate(g, CardPlacementController.instance.opponentNegativeArea);
                            }
                        }
                        else if(target == "player")
                        {
                            if (g.GetComponent<NumberStats>().positive)
                            {
                                Instantiate(g, CardPlacementController.instance.playerPositiveArea);
                            }
                            else
                            {
                                Instantiate(g, CardPlacementController.instance.playerNegativeArea);
                            }
                        }
                    }
                    else if(toDo == "change")
                    {
                        if(g.transform.parent == CardPlacementController.instance.playerPositiveArea || g.transform.parent == CardPlacementController.instance.playerNegativeArea)
                        {
                            StartCoroutine(ChangeNumber(g, x, "player"));
                            
                        }
                        else
                        {
                            StartCoroutine(ChangeNumber(g, x, "opponent"));
                            
                        }
                    }
                    else if(toDo == "changeBait")
                    {
                        if(g.GetComponent<NumberStats>().value == 2)
                        {
                            StartCoroutine(ChangeNumber(g, -2, "opponent"));

                        }
                        else
                        {
                            StartCoroutine(ChangeNumber(g, 2, "opponent"));
                        }
                    }
                    else if(toDo == "give")
                    {
                        StartCoroutine(GiveNumber(g, target));

                        
                    }

                    Debug.Log("execute action");
                    choiceObj.SetActive(false);
                });



            }
        }

        //reset all selectable
        foreach(GameObject g in combined)
        {
            g.GetComponent<NumberStats>().selectable = false;
        }


    }

    IEnumerator GiveNumber(GameObject g, string target)
    {
        StartCoroutine(DeleteShader(g));
        yield return new WaitForSeconds(1f);

        Destroy(g.GetComponent<CardPlace>().correspondingImage);
        


        if (target == "opponent")
        {
            if (g.GetComponent<NumberStats>().positive)
            {
                Instantiate(g, CardPlacementController.instance.opponentPositiveArea);
            }
            else
            {
                Instantiate(g, CardPlacementController.instance.opponentNegativeArea);
            }
        }
        else if (target == "player")
        {
            if (g.GetComponent<NumberStats>().positive)
            {
                Instantiate(g, CardPlacementController.instance.playerPositiveArea);
            }
            else
            {
                Instantiate(g, CardPlacementController.instance.playerNegativeArea);
            }
        }

        Destroy(g);
        yield return new WaitForSeconds(0.7f);
    }

    public IEnumerator ChangeNumber(GameObject g, int x, string target)
    {
        StartCoroutine(DeleteShader(g));
        yield return new WaitForSeconds(1f);

        Destroy(g.GetComponent<CardPlace>().correspondingImage);
        Destroy(g);
        yield return new WaitForSeconds(0.7f);
        SpecialCardManager.instance.Give(x, target);
    }

    public IEnumerator DiscardNumber(GameObject g)
    {
        StartCoroutine(DeleteShader(g));
        yield return new WaitForSeconds(1f);

        Destroy(g.GetComponent<CardPlace>().correspondingImage);
        Destroy(g);
        yield return new WaitForSeconds(0.7f);
    }



    IEnumerator SwapOut(GameObject g, string target)
    {
        StartCoroutine(DeleteShader(g));
        yield return new WaitForSeconds(1f);


        Destroy(g.GetComponent<CardPlace>().correspondingImage);
        Destroy(g);
        yield return new WaitForSeconds(0.7f);
        CardPlacementController.instance.DealOneCard(target);
        yield return new WaitForSeconds(0.7f);
    }

    IEnumerator DeleteShader(GameObject g)
    {
        RawImage rawImage = g.GetComponent<CardPlace>().correspondingImage.GetComponent<RawImage>();

        Material mat = new Material(rawImage.materialForRendering);

        rawImage.material = mat;

        float startFade = mat.GetFloat("_Fade");
        float endFade = 0f;

        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            float lerpVal = Mathf.Lerp(startFade, endFade, t / 1f);
            mat.SetFloat("_Fade", lerpVal);
            yield return null;
        }

        mat.SetFloat("_Fade", endFade);

        yield return new WaitForSeconds(0.5f);
    }
}
