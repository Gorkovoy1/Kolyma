using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PrologueStepData : MonoBehaviour
{
    public string message;
    public string speaker;
    public Action afterContinue;
    public Func<bool> waitUntil;
    public float autoAdvanceDelay = 0.3f;
    public bool requireContinue = true;
    public bool dialogue = true;
    public GameObject dialogueBox;
    public bool activateNextCard = false;
}
