using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeVisualController : MonoBehaviour
{
    public Image yellowFill;
    public Image blueFill;
    public Image redFill;

    public float yellowFillAmount;
    public float blueFillAmount;
    public float redFillAmount;

    public float gaugeSpeed = 4f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
