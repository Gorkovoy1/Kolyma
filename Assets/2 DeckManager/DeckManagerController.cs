using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManagerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CardInventoryController.instance.ManageDeck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
