using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkerController : MonoBehaviour
{
    public int index;
    public GameObject[] dates;

    // Start is called before the first frame update
    void Start()
    {
        index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = dates[index].transform.position;
    }

    public void GoNext()
    {
        if(index == dates.Length-1)
        {
            Debug.Log("last date");
        }
        else
        {
            index++;
        }
    }
}
