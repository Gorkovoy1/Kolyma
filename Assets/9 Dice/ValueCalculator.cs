using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ValueCalculator : MonoBehaviour
{
    public GameObject dice1;
    public GameObject dice2;
    public GameObject dice3;
    public GameObject dice4;

    public Transform diceParent;


    // Start is called before the first frame update
    void Start()
    {
        dice1 = diceParent.GetChild(0).gameObject;
        dice2 = diceParent.GetChild(1).gameObject;
        dice3 = diceParent.GetChild(2).gameObject;
        dice4 = diceParent.GetChild(3).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(dice1 != null)
        {
            this.gameObject.GetComponent<TextMeshProUGUI>().text = "" + (dice1.GetComponent<DiceController>().topVal + dice2.GetComponent<DiceController>().topVal + dice3.GetComponent<DiceController>().topVal + dice4.GetComponent<DiceController>().topVal);
        }
        

        if(Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
