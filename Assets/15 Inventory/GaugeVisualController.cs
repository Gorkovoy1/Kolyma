using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GaugeVisualController : MonoBehaviour
{
    public Image yellowFill;
    public Image blueFill;
    public Image redFill;

    public float yellowFillAmount;
    public float blueFillAmount;
    public float redFillAmount;

    public float gaugeSpeed = 4f;

    [SerializeField] TextMeshProUGUI yellowAmount;
    [SerializeField] TextMeshProUGUI blueAmount;
    [SerializeField] TextMeshProUGUI redAmount;

    void Start()
    {
        yellowFill.fillAmount = GaugeController.instance.hunger / 100f; 
        blueFill.fillAmount = GaugeController.instance.weakness / 100f; 
        redFill.fillAmount = GaugeController.instance.cold / 100f; 
    }

    

    // Update is called once per frame
    void Update()
    {
        if(yellowAmount != null)
        {
            yellowAmount.text = GaugeController.instance.hunger + "%";
            blueAmount.text = GaugeController.instance.cold + "%";
            redAmount.text = GaugeController.instance.weakness + "%";
        }
        

        float targetYellow = GaugeController.instance.hunger / 100f;
        float targetRed = GaugeController.instance.weakness / 100f;
        float targetBlue = GaugeController.instance.cold / 100f;

        yellowFillAmount = Mathf.Lerp(yellowFillAmount, targetYellow, Time.deltaTime * gaugeSpeed);
        redFillAmount = Mathf.Lerp(redFillAmount, targetRed, Time.deltaTime * gaugeSpeed);
        blueFillAmount = Mathf.Lerp(blueFillAmount, targetBlue, Time.deltaTime * gaugeSpeed);

        yellowFill.fillAmount = yellowFillAmount;
        blueFill.fillAmount = blueFillAmount;
        redFill.fillAmount = redFillAmount;
        
    }
}
