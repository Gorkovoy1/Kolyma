using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberManager : MonoBehaviour
{
    public List<GameObject> allNumbers = new List<GameObject>();
    public List<GameObject> positives = new List<GameObject>();
    public List<GameObject> negatives = new List<GameObject>();
    public List<GameObject> yellows = new List<GameObject>();
    public List<GameObject> blues = new List<GameObject>();
    public List<GameObject> reds = new List<GameObject>();

    public List<GameObject> duplicates = new List<GameObject>();
    public List<GameObject> flipped = new List<GameObject>();
    public List<GameObject> given = new List<GameObject>();
    public List<GameObject> swapped = new List<GameObject>();
    public List<GameObject> discarded = new List<GameObject>();



    public List<GameObject> OPPallNumbers = new List<GameObject>();
    public List<GameObject> OPPpositives = new List<GameObject>();
    public List<GameObject> OPPnegatives = new List<GameObject>();
    public List<GameObject> OPPyellows = new List<GameObject>();
    public List<GameObject> OPPblues = new List<GameObject>();
    public List<GameObject> OPPreds = new List<GameObject>();

    public List<GameObject> OPPduplicates = new List<GameObject>();
    public List<GameObject> OPPflipped = new List<GameObject>();
    public List<GameObject> OPPgiven = new List<GameObject>();
    public List<GameObject> OPPswapped = new List<GameObject>();
    public List<GameObject> OPPdiscarded = new List<GameObject>();

    public GameObject oppPositiveArea;
    public GameObject oppNegativeArea;
    public GameObject playerPositiveArea;
    public GameObject playerNegativeArea;

    public bool recalculate;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(recalculate)
        {
            recalculate = false;
            RecalculateNumberGroups();
        }
    }

    public void RecalculateNumberGroups()
    {
        //
        RecalculateAllNumbers();
        RecalculatePlayerPositives();
        RecalculateOpponentPositives();
        RecalculatePlayerNegatives();
        RecalculateOpponentNegatives();
        RecalculateAllBlues();
        RecalculateAllReds();
        RecalculateAllYellows();
    }

    void RecalculateAllYellows()
    {
        yellows = new List<GameObject>();
        foreach (var v in allNumbers)
        {
            if (v.GetComponent<NumberStats>().yellow)
            {
                yellows.Add(v);
            }
        }

        OPPyellows = new List<GameObject>();
        foreach (var v in OPPallNumbers)
        {
            if (v.GetComponent<NumberStats>().yellow)
            {
                OPPyellows.Add(v);
            }
        }
    }

    void RecalculateAllBlues()
    {
        blues = new List<GameObject>();
        foreach(var v in allNumbers)
        {
            if(v.GetComponent<NumberStats>().blue)
            {
                blues.Add(v);
            }
        }

        OPPblues = new List<GameObject>();
        foreach(var v in OPPallNumbers)
        {
            if(v.GetComponent<NumberStats>().blue)
            {
                OPPblues.Add(v);
            }
        }
    }

    void RecalculateAllReds()
    {
        reds = new List<GameObject>();
        foreach (var v in allNumbers)
        {
            if (v.GetComponent<NumberStats>().red)
            {
                reds.Add(v);
            }
        }

        OPPreds = new List<GameObject>();
        foreach (var v in OPPallNumbers)
        {
            if (v.GetComponent<NumberStats>().red)
            {
                OPPreds.Add(v);
            }
        }
    }

    void RecalculateAllNumbers()
    {
        //player
        allNumbers = new List<GameObject>();
        foreach(Transform child in playerPositiveArea.transform)
        {
            allNumbers.Add(child.gameObject);
        }
        foreach (Transform child in playerNegativeArea.transform)
        {
            allNumbers.Add(child.gameObject);
        }
        //opponent
        OPPallNumbers = new List<GameObject>();
        foreach (Transform child in oppPositiveArea.transform)
        {
            OPPallNumbers.Add(child.gameObject);
        }
        foreach (Transform child in oppNegativeArea.transform)
        {
            OPPallNumbers.Add(child.gameObject);
        }
    }

    void RecalculatePlayerPositives()
    {
        positives = new List<GameObject>();


        foreach (Transform child in playerPositiveArea.transform)
        {
            positives.Add(child.gameObject);
        }
    }

    void RecalculateOpponentPositives()
    {
        OPPpositives = new List<GameObject>();

        foreach(Transform child in oppPositiveArea.transform)
        {
            OPPpositives.Add(child.gameObject);
        }
    }

    void RecalculatePlayerNegatives()
    {
        negatives = new List<GameObject>();


        foreach (Transform child in playerNegativeArea.transform)
        {
            negatives.Add(child.gameObject);
        }
    }

    void RecalculateOpponentNegatives()
    {
        OPPnegatives = new List<GameObject>();

        foreach (Transform child in oppNegativeArea.transform)
        {
            OPPnegatives.Add(child.gameObject);
        }
    }
}
