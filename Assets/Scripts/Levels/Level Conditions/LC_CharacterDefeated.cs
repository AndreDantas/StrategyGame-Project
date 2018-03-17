using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LC_CharacterDefeated : LevelCondition
{

    public Character character;

    public override bool IsComplete(BattleController unused)
    {
        if (character == null)
            return false;
        return character.IsDown();
    }


}
