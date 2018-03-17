using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBattleState : BattleState
{

    public override void Enter()
    {
        base.Enter();
        StartCoroutine(EndBattle());
    }

    IEnumerator EndBattle()
    {
        yield return null;
    }
}
