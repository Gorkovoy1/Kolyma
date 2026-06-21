using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndWeekController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndWeek()
    {
        StartCoroutine(SceneLoader.instance.LoadNextScene("7 BasicCalendar"));
    }

    public void BackToHub()
    {
        StartCoroutine(SceneLoader.instance.LoadNextScene("0.5 Hub"));
    }
}
