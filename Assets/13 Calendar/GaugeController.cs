using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeController : MonoBehaviour
{
    public int cold;
    public int hunger;
    public int weakness;

    public int coldIncrement;
    public int hungerIncrement;
    public int weaknessIncrement;

    public static GaugeController instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }


        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PassTime()
    {
        AddCold();
        AddHunger();
        AddWeakness();
    }

    public void AddCold() 
    {
        cold += coldIncrement;
    }

    public void AddHunger()
    {
        hunger += hungerIncrement;
    }

    public void AddWeakness()
    {
        weakness += weaknessIncrement;
    }

    public void ReplenishCold(int amount)
    {
        cold -= amount;
    }

    public void ReplenishHunger(int amount)
    {
        hunger -= amount;
    }

    public void ReplenishWeakness(int amount)
    {
        weakness -= amount;
    }
}
