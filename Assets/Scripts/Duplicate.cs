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
    
}
