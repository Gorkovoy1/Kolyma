using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DestroyOnClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("click");
        Destroy(this.gameObject);
    }

    public void Destroy()
    {
        Destroy(this);
        Destroy(gameObject);
        Destroy(this.gameObject);
    }
}
