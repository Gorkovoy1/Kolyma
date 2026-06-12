using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreditsTextController : MonoBehaviour
{
    public string[] lines;
    public TextMeshProUGUI creditsText;
    public int index = 0;
    public bool creditsEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        lines = new string[] { "Shapes and Colors Gaming\npresents", "Story by\nAndrei Gorkovoi", "Programming by\nJasmine Ding", "with help from\nMark Ilog\nElizabeth Arnold", "Art by\nTiana Oreglia\nEvgeniya Shmeleva", "Music and Sound Design\nby\nAndrei Gorkovoi" };
        creditsText.text = lines[index];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextLine()
    {
        index++;
        creditsText.text = lines[index];
    }

    public void ResetArray()
    {
        index = 0;
        creditsText.text = lines[index];
    }

    public void CreditsEnd()
    {
        creditsEnd = true;
        this.GetComponentInParent<RecordController>().gameObject.SetActive(false);
    }
}
