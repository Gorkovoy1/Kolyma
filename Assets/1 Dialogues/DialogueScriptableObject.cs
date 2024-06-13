using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "DialogueData", menuName = "DialogueScriptables", order = 1)]

public class DialogueScriptableObject : ScriptableObject
{
    public Sprite bg;
    public Sprite npcBlack;
    public Sprite npcColor;
    public string line1;
    public string line2;
    public string music;
    public TextAsset inkfile;
    public int nextScene;
    public GameObject ambient;
    public GameObject bganim;
    public GameObject sfx;
}
