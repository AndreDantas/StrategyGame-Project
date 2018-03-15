using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LC_AllAlliesDefeated : LevelCondition
{
    public LC_AllAlliesDefeated(string description = "All allies defeated.")
    {
        this.description = description;
    }

    public LC_AllAlliesDefeated()
    {
        description = "All allies defeated.";
    }

    public override bool IsComplete(BattleController battleController)
    {
        List<Character> characters = battleController.activeUnits;
        bool result = true;
        foreach (Character c in characters)
        {
            if (LayerMask.LayerToName(c.gameObject.layer) == "Player")
            {
                result = false;
                break;
            }

        }

        return result;
    }
}
