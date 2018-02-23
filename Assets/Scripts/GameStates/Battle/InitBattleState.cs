using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitBattleState : BattleState
{

    public override void Enter()
    {
        base.Enter();
        StartCoroutine(Init());
    }
    IEnumerator Init()
    {
        //Actions before battle.

        yield return null;
        owner.ChangeState<MoveTargetState>();
    }
}
