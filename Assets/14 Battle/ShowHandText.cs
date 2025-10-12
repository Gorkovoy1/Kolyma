using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ShowHandText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject showText;
    // Start is called before the first frame update
    void Start()
    {
        showText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        showText.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        showText.SetActive(false);
    }
}
