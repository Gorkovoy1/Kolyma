using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int value;
    public bool discard;
    public bool flipped;

    public bool action;

    public static PlayerStats instance;

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
