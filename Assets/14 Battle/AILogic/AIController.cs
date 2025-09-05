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
        selectedCardToPlay.GetComponent<AICardPlace>().AnimateBeingPlayed();
    }
}




