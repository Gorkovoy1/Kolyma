using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Dice : MonoBehaviour 
{

    public Sprite[] diceSides;
    public int finalSide;

    // Reference to sprite renderer to change sprites
    private Image img;


    // Coroutine that rolls the dice
    public IEnumerator RollTheDice(float time)
    {
        img = GetComponent<Image>();
        // Variable to contain random dice side number.
        // It needs to be assigned. Let it be 0 initially
        int randomDiceSide = 0;

        // Loop to switch dice sides ramdomly
        // before final side appears. 20 itterations here.
        for (int i = 0; i <= 20; i++)
        {
            // Pick up random value from 0 to 5 (All inclusive)
            randomDiceSide = Random.Range(0, 6);

            // Set image to upper face of dice from array according to random value
            img.sprite = diceSides[randomDiceSide];

            // Pause before next itteration
            yield return new WaitForSeconds(time/20f);
        }

        // Assigning final side so you can use this value later in your game
        // for player movement for example
        finalSide = randomDiceSide + 1;
    }
}
