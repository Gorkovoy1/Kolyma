using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberStats : MonoBehaviour
{
    public bool positive;
    public bool negative;
    public bool blue;
    public bool red;
    public bool yellow;

    public bool flip;

    public int value;

    public bool selectable;

    // Start is called before the first frame update
    void Start()
    {
        flip = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if positive but parent is negative 
        //if negative but parent is positive
        //then flip = true
    }
}
