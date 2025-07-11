using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialStepData 
{
    public string message;
    public Action afterContinue;
    public Func<bool> waitUntil;
    public float autoAdvanceDelay = 0.8f;
    public bool requireContinue = true;
}
