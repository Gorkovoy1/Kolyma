using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI Evaluation Values")]
public class AIEvaluationValues : ScriptableObject
{
    public int PlayerAtTarget, PlayerOverTarget;
    public int OpponentAtTarget, OpponentOverTarget;
}
