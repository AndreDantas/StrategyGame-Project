using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultLevelDetails : LevelDetails
{

    protected override void Start()
    {
        base.Start();
        winConditions.Add(new LC_AllEnemiesDefeated());
        failConditions.Add(new LC_AllAlliesDefeated());
        failConditions.Add(new LC_ReachTurn(30));
    }
}
