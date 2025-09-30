using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountingMatController : MonoBehaviour
{

    public bool matCreated = false;
    public GameObject countingBoxGreen;
    public GameObject countingBoxRed;
    public Transform playerMatTwo;
    public Transform oppMatTwo;
    public Transform playerMat;
    public Transform oppMat;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!matCreated && NumberManager.instance.recalculate)
        {
            matCreated = true;
            //clear the boxes
            foreach (Transform child in oppMat)
            {
                Destroy(child.gameObject);
                Debug.Log("destroy");
            }
            foreach (Transform child in oppMatTwo)
            {
                Destroy(child.gameObject);
            }
            //if positives exceed target, just instantiate target number and extra reds
            if (NumberManager.instance.oppPosVal >= NumberManager.instance.targetVal)
            {
                for (int i = 0; i < NumberManager.instance.targetVal; i++)
                {
                    Instantiate(countingBoxGreen, oppMat);
                    Debug.Log("spawn green");
                }
                for (int i = 0; i < NumberManager.instance.oppPosVal - NumberManager.instance.targetVal; i++)
                {
                    Instantiate(countingBoxRed, oppMat);
                }
                for (int i = 0; i < 8 - NumberManager.instance.oppPosVal - NumberManager.instance.targetVal; i++)
                {
                    Instantiate(countingBoxRed, oppMatTwo);
                }
            }
            //if positives dont exceed target, instantiate pos, then target - pos, then reds
            else
            {
                for (int i = 0; i < NumberManager.instance.oppPosVal; i++)
                {
                    Instantiate(countingBoxGreen, oppMat);
                }
                for (int i = 0; i < NumberManager.instance.targetVal - NumberManager.instance.oppPosVal; i++)
                {
                    Instantiate(countingBoxGreen, oppMatTwo);
                }
                for (int i = 0; i < 8; i++)
                {
                    Instantiate(countingBoxRed, oppMatTwo);
                }
            }

            //clear the boxes
            foreach (Transform child in playerMat)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in playerMatTwo)
            {
                Destroy(child.gameObject);
            }
            //if positives exceed target, just instantiate target number and extra reds
            if (NumberManager.instance.playerPosVal >= NumberManager.instance.targetVal)
            {
                for (int i = 0; i < NumberManager.instance.targetVal; i++)
                {
                    Instantiate(countingBoxGreen, playerMat);
                }
                for (int i = 0; i < NumberManager.instance.playerPosVal - NumberManager.instance.targetVal; i++)
                {
                    Instantiate(countingBoxRed, playerMat);
                }
                for (int i = 0; i < 8 - NumberManager.instance.playerPosVal - NumberManager.instance.targetVal; i++)
                {
                    Instantiate(countingBoxRed, playerMatTwo);
                }
            }
            //if positives dont exceed target, instantiate pos, then target - pos, then reds
            else
            {
                for (int i = 0; i < NumberManager.instance.playerPosVal; i++)
                {
                    Instantiate(countingBoxGreen, playerMat);
                }
                for (int i = 0; i < NumberManager.instance.targetVal - NumberManager.instance.playerPosVal; i++)
                {
                    Instantiate(countingBoxGreen, playerMatTwo);
                }
                for (int i = 0; i < 8; i++)
                {
                    Instantiate(countingBoxRed, playerMatTwo);
                }
            }

            matCreated = false;

        }
    }
}
