using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Duplicate : MonoBehaviour
{
    public GameManager gm;
    public GameObject cardPrefab;
    public GameObject selectedCard;

    // Start is called before the first frame update
    private void Start()
    {
        GameObject manager = GameObject.Find("Game Manager");    
        gm = manager.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
    public void DuplicateCard()
    {
        
        if (Input.GetMouseButtonDown(0)) // If the left mouse button is pressed
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.tag == "AIValue") // If a card is clicked
            {
                selectedCard = hit.collider.gameObject; // Set the selected card

                // Deselect any previously selected card
                foreach (Transform child in transform)
                {
                    child.GetComponent<Image>().color = Color.white;
                }

                // Highlight the selected card
                selectedCard.GetComponent<Image>().color = Color.yellow;


            }
        }

        if (Input.GetKeyDown(KeyCode.D) && selectedCard != null) // If the "D" key is pressed and a card is selected
        {
            GameObject newCard = (GameObject) Instantiate (cardPrefab); // Create a new card at the position of the selected card

            if(newCard.GetComponent<ValueCard>().value > 0)
            {
                gm.AIValues.Add(newCard);
            }
            if(newCard.GetComponent<ValueCard>().value < 0)
            {
                gm.tempNegsAI.Add(newCard);
            }

            gm.OrganizeCards();
        }
    }
    */


}
