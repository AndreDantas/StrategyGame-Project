using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : BattleState
{
    public static float MAX_WAIT_TIME = 180;
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
        float counter = 0;
        while (!ai.IsDone() || counter > MAX_WAIT_TIME)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        owner.ChangeState<SelectTargetState>();

    }
}
