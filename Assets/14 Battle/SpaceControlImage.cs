using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceControlImage : MonoBehaviour
{
    public GameObject correspondingImage;
    public Transform numberDeckTransform;

    // Start is called before the first frame update
    void Start()
    {
        GameObject parent = this.gameObject.transform.parent.gameObject;
        GameObject parent2 = parent.transform.parent.gameObject;
        GameObject parent3 = parent2.transform.parent.gameObject;

        numberDeckTransform = parent3.GetComponentInChildren<NumberDeck>().gameObject.transform;


        GameObject card = Instantiate(correspondingImage, numberDeckTransform);
        card.GetComponent<AnimatorScript>().target = this.gameObject.GetComponent<RectTransform>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
