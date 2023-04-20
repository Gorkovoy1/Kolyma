using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    SpriteRenderer spriteRenderer; 

    public List<Sprite> faces;
    public Sprite cardBack;
    public int cardIndex;

    public int newIndex;
    
    void Start()
    {
        //on start, produces a shuffled list of sprites

        for(int i = 0; i < 40; i++)
        {
            Sprite temp = faces[i];
            int rand = Random.Range(0,40);
            faces[i] = faces[rand];
            faces[rand] = temp;
        }

        
    }



    
}
