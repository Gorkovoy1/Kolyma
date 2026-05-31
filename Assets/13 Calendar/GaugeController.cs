using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GaugeController : MonoBehaviour
{
    public int cold;
    public int hunger;
    public int weakness;

    public int coldIncrement;
    public int coldChange;
    public int hungerIncrement;
    public int hungerChange;
    public int weaknessIncrement;
    public int weaknessChange;

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
        coldChange = coldIncrement / 7;
        hungerChange = hungerIncrement / 7;
        weaknessChange = weaknessIncrement / 7;
    }

    // Update is called once per frame
    void Update()
    {
        if(cold >= 100 || hunger >= 100 || weakness >= 100 && SceneManager.GetActiveScene().name != "2 YouLose")
        {
            //trigger lose scene
            StartCoroutine(SceneLoader.instance.LoadNextScene("2 YouLose"));
        }
    }

    public void PassTime(int daysToAdvance)
    {
        AddCold(daysToAdvance * coldChange);
        AddHunger(daysToAdvance * hungerChange);
        AddWeakness(daysToAdvance * weaknessChange);
    }

    public void AddCold(int amount) 
    {
        cold += amount;
    }

    public void AddHunger(int amount)
    {
        hunger += amount;
    }

    public void AddWeakness(int amount)
    {
        weakness += amount;
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
