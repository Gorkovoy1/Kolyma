using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelectionController : MonoBehaviour
{
    public bool selectable;
    public bool selected;

    // Start is called before the first frame update
    void Start()
    {
        selectable = false;
        selected = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(selectable)
        {
            //light up!
            //activate button component
            //on click make all objects unselectable
            //set selected (other script looks for selected and uses that card for script)
        }
    }


}
