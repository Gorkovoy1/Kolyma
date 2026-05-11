using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceButtonController : MonoBehaviour
{
    public int itemsAvailable;
    public GameObject[] resourcePool;

    // Start is called before the first frame update
    void Start()
    {
        //clear items
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }

        //spawn items
        for (int i = 0; i < itemsAvailable; i++)
        {
            Instantiate(resourcePool[Random.Range(0, resourcePool.Length)], this.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
