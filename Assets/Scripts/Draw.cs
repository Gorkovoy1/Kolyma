using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour
{

    public GameManager gm;

    // Start is called before the first frame update
    private void Start()
    {
        GameObject manager = GameObject.Find("Game Manager");    
        gm = manager.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void if2DrawSpecial()
    {
        GameObject two = gm.AIValues.Find(obj => obj.GetComponent<ValueCard>().value == 2);
        if(two != null)
        {
            gm.DrawSpecial();
        }
    }

    public void Draw2Special()
    {
        gm.DrawSpecial();
        gm.DrawSpecial();
    }
    
}
