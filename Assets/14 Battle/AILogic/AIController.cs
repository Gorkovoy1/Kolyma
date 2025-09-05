using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AIController : MonoBehaviour
{

    public GameObject selectedCardToPlay;
    public Difficulty difficulty;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayCard()
    {
        selectedCardToPlay.GetComponent<AICardPlace>().difficulty = difficulty;

        //check if selected card will bust you, if itll bust you then dont play it, just skip turn ?? how to check if only happens after you play card though


        selectedCardToPlay.GetComponent<AICardPlace>().AnimateBeingPlayed();
    }
}




