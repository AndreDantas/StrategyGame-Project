using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LC_AllEnemiesDefeated : LevelCondition
{

    public LC_AllEnemiesDefeated(string description = "All enemies defeated.")
    {
        this.description = description;
    }

    public LC_AllEnemiesDefeated()
    {
        description = "All enemies defeated.";
    }

    public override bool IsComplete(BattleController battleController)
    {
        List<Character> characters = battleController.activeUnits;
        bool result = true;
        foreach (Character c in characters)
        {
            if (LayerMask.LayerToName(c.gameObject.layer) == "Enemy")
            {
                result = false;
                break;
            }

        }

        return result;
    }
}
