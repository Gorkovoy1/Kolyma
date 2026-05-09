using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<GameObject> breadList;
    public List<GameObject> drinkList;
    public List<GameObject> clothesList;
    public List<GameObject> otherList;

    public int breadIndex = 0;
    public int drinkIndex = 0;
    public int clothesIndex = 0;
    public int otherIndex = 0;

    public static InventoryManager instance;

    public GameObject slotPrefab;

    public GameObject[] betSlotArray;

    public GameObject consumeArea;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddNewItem(GameObject objToAdd, List<GameObject> list, int index)
    {
        foreach(GameObject g in list)
        {
            if(g.transform.childCount == 0)
            {
                index = list.IndexOf(g);
                break;
            }
            else
            {
                index = -1;
            }
        }
        if(index == -1)
        {
            //add one more slot
            GameObject newSlot = Instantiate(slotPrefab, list[0].transform.parent);
            list.Add(newSlot);
            index = list.IndexOf(newSlot);
        }

        objToAdd.transform.SetParent(list[index].transform);
        objToAdd.transform.localPosition = Vector3.zero;
    }


}
