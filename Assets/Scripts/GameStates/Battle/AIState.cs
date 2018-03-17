using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : BattleState
{
    public override void Enter()
    {
        base.Enter();
        StartCoroutine(AITurn());
    }

    IEnumerator AITurn()
    {
        AIController ai = turn.actor.GetComponent<AIController>();
        if (ai == null)
        {
            owner.ChangeState<SelectTargetState>();
            yield break;
        }

        ai.ExecuteTurn();
        while (!ai.IsDone())
        {
            yield return null;
        }

        owner.ChangeState<SelectTargetState>();

    }
}
