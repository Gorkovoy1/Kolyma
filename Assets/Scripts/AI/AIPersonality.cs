using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI Personality")]
public class AIPersonality : ScriptableObject
{
    public enum Difficulty { ReallyHard, Hard, Normal, Easy, ReallyEasy }
    public enum AIPreference { NOT_SET,  Always, Loves, Likes, Neutral, Dislikes, Hates, Never}

    public Difficulty AIDifficulty;

    public AIPreference AIAtTarget, OpponentAtTarget;
    public AIPreference AIOverTarget, OpponentOverTarget;
    public AIPreference AIDrawsACard, OpponentDrawsACard;
    public AIPreference AIDiscardsACard, OpponentDiscardsACard;
    public AIPreference AISwapsACard, OpponentSwapsACard;
    public AIPreference AIFlipsACard, OpponentFlipsACard;

    public static int GetPriorityValue(AIPreference preference)
    {
        switch(preference)
        {
            case AIPreference.Never:
                return 100;
            case AIPreference.Hates:
                return 4;
            case AIPreference.Dislikes:
                return 2;
            case AIPreference.Neutral:
                return 0;
            case AIPreference.Likes:
                return -2;
            case AIPreference.Loves:
                return -4;
            case AIPreference.Always:
                return -100;
            default:
                return 0;
        }
    }
}
