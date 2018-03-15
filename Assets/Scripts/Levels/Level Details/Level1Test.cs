using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Test : LevelDetails
{
    protected override void Start()
    {
        base.Start();
        winConditions.Add(new LC_AllEnemiesDefeated());
        failConditions.Add(new LC_AllAlliesDefeated());
        failConditions.Add(new LC_ReachTurn("Exceed 20 turns.", 20));
    }

}
