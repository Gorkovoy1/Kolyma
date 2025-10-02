using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public bool isPlayerTurn;
    public static TurnManager instance;
    public bool checkedPlayable = false;

    public bool opponentPlayedCard = false;
    public bool playerPlayedCard = false;
    // Start is called before the first frame update

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchTurn()
    {
        isPlayerTurn = !isPlayerTurn;
    }
}
