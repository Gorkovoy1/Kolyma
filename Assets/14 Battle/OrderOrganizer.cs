using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class OrderOrganizer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSortingOrder();
    }

    void UpdateSortingOrder()
    {
        List<RectTransform> children = new List<RectTransform>();

        // Collect all children (UI elements with Image components or any Renderable objects)
        Transform parentTransform = this.transform;
        for (int i = 0; i < parentTransform.childCount; i++)
        {
            RectTransform rect = parentTransform.GetChild(i).GetComponent<RectTransform>();
            if (rect != null)
            {
                children.Add(rect);
            }
        }

        // Sort children by X position (left to right)
        children.Sort((a, b) => a.anchoredPosition.x.CompareTo(b.anchoredPosition.x));


        // Assign sorting order based on position
        for (int i = 0; i < children.Count; i++)
        {
            if(children[i].anchoredPosition.y < -300)
            {
                children[i].gameObject.transform.SetSiblingIndex(children.Count - i);
            }
            else if(children[i].anchoredPosition.y < -200)
            {
               
                children[i].gameObject.transform.SetSiblingIndex(i);
                
            }



        }
    }
}
