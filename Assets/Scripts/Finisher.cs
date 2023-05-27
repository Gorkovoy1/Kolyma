using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finisher : MonoBehaviour
{
    
    public GameObject four;
    public GameManager gm;
    public Transform board;
    public TurnSystem ts;
    
    public bool attack;

    // Start is called before the first frame update
    private void Start()
    {
        GameObject manager = GameObject.Find("Game Manager");    
        gm = manager.GetComponent<GameManager>();
        GameObject tempBoard = GameObject.Find("Canvas/Panel");
        board = tempBoard.transform; 
        attack = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwapFinisher()
    {
        for(int i = 0; i < gm.cardsSwapped; i++)
        {
            GameObject givePrefab = (GameObject) Instantiate (four, board);
            givePrefab.transform.SetParent(board, false);
            gm.AIValues.Add(givePrefab);
            gm.OrganizeCards();
            gm.gameEnd = true;
        }
        DestroyMe();
    }

    public void DiscardFinisher()
    {
        for(int i = 0; i < gm.cardsDiscarded; i++)
        {
            GameObject givePrefab = (GameObject) Instantiate (four, board);
            givePrefab.transform.SetParent(board, false);
            gm.AIValues.Add(givePrefab);
            gm.OrganizeCards();
            gm.gameEnd = true;
        }
        DestroyMe();
    }


    public void GiveFinisher()
    {
        for(int i = 0; i < gm.cardsGiven; i++)
        {
            GameObject givePrefab = (GameObject) Instantiate (four, board);
            givePrefab.transform.SetParent(board, false);
            gm.AIValues.Add(givePrefab);
            gm.OrganizeCards();
            gm.gameEnd = true;
        }
        DestroyMe();
    }

    void DestroyMe()
    {
        
        gm.playerSpecials.Remove(this.gameObject);
        Destroy(this.gameObject);
        gm.playerSpecials.Remove(this.gameObject);
        Destroy(this.gameObject);
        GameObject manager = GameObject.Find("Game Manager");    
        ts = manager.GetComponent<TurnSystem>();
        ts.cardSelected = true;
    }
}
