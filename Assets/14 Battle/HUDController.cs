using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public TextMeshProUGUI playerVal;
    public TextMeshProUGUI oppVal;
    public TextMeshProUGUI targetVal;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerVal.text = NumberManager.instance.playerVal.ToString();
        oppVal.text = NumberManager.instance.oppVal.ToString();
        targetVal.text = NumberManager.instance.targetVal.ToString();
    }
}
