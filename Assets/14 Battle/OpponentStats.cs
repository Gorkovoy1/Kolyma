using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentStats : MonoBehaviour
{
    public int value;
    public bool discarded;
    public bool flipped;
    public bool swapped;
    public bool gave;

    public bool action;

    public static OpponentStats instance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
