using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DragResourcesText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI consumeText;
    [SerializeField] TextMeshProUGUI dragResourcesText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(UIPanelManager.instance.currentState == UIState.Inventory)
        {
            dragResourcesText.gameObject.SetActive(!consumeText.gameObject.activeInHierarchy);
        }
        else
        {
            dragResourcesText.gameObject.SetActive(false);
        }
        
    }
}
