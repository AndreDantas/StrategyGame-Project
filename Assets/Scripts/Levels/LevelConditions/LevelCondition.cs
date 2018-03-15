using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class LevelCondition
{

    public string description;
    public abstract bool IsComplete(BattleController battleController);

    public static bool AllConditionsMet(List<LevelCondition> conditions, BattleController battleController)
    {
        bool result = true;
        foreach (LevelCondition l in conditions)
        {
            if (!l.IsComplete(battleController))
            {
                result = false;
                break;
            }
        }
        return result;
    }

    public static bool AnyConditionMet(List<LevelCondition> conditions, BattleController battleController)
    {
        bool result = false;

        foreach (LevelCondition l in conditions)
        {
            if (l.IsComplete(battleController))
            {
                result = true;
                break;
            }
        }
        return result;
    }
}
