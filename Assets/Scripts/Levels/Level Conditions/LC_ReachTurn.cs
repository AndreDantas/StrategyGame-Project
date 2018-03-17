using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LC_ReachTurn : LevelCondition
{

    public int turnCount = 20;
    public LC_ReachTurn(string description, int turnCount = 20)
    {
        turnCount = MathOperations.ClampMin(turnCount, 1);
        this.turnCount = turnCount;
        this.description = description;
    }
    public LC_ReachTurn(int turnCount = 20)
    {
        description = "Reachs turn 20";
        turnCount = MathOperations.ClampMin(turnCount, 1);
        this.turnCount = turnCount;
    }
    public override bool IsComplete(BattleController battleController)
    {
        return (battleController.turn.turnCount > turnCount);
    }
}
