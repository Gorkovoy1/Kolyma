using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GaugeValueUpdater : MonoBehaviour
{
    public TextMeshProUGUI coldText;
    public TextMeshProUGUI hungerText;
    public TextMeshProUGUI weaknessText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        coldText.text = "COLD                   " + GaugeController.instance.cold + "%";
        hungerText.text = "HUNGER            " + GaugeController.instance.hunger + "%";
        weaknessText.text = "WEAKNESS       " + GaugeController.instance.weakness + "%";
    }
}
