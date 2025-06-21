using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialStep : MonoBehaviour
{
    public string message;
    public Action afterContinue;
    public Func<bool> waitUntil;
    public float autoAdvanceDelay = 0.5f;
    public bool requireContinue = true;
}
