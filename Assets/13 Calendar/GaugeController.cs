using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeController : MonoBehaviour
{
    public Slider cold;
    public Slider hunger;
    public Slider weakness;

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

    public void AddCold()
    {
        cold.value += coldIncrement;
    }

    public void AddHunger()
    {
        hunger.value += hungerIncrement;
    }

    public void AddWeakness()
    {
        weakness.value += weaknessIncrement;
    }
}
