using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBattleState : BattleState
{

    public override void Enter()
    {
        base.Enter();
        cameraControl.canMove = false;
        StartCoroutine(EndBattle());
    }

    IEnumerator EndBattle()
    {
        yield return null;

        //Battle end scene

        owner.ChangeState<RewardsState>();

    }
}
