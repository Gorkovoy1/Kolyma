using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject description;
    Material normal;
    public Material border;

    private Image imageComponent;
    private Material originalMaterial; // Store the original material to revert back

    void Start()
    {
        description.SetActive(false);
        imageComponent = GetComponent<Image>();
        originalMaterial = imageComponent.material;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("enter!");
        description.SetActive(true);
        GetComponent<Image>().material = border;
        description.transform.SetAsLastSibling(); // Bring the description to the front
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("exit!");
        description.SetActive(false);
        GetComponent<Image>().material = normal;
    }
}