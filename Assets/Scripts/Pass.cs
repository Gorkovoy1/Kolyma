using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pass : MonoBehaviour
{
    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        
        GameObject manager = GameObject.Find("Game Manager");
        gm = manager.GetComponent<GameManager>();

    }

    public void PassTurn()
    {
        
        gm.cardSelected = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
