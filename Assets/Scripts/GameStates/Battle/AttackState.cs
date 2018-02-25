using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackState : BattleState
{


    public override void Enter()
    {
        base.Enter();
        DeactivateSelectNode();
        StartCoroutine(ExecuteAttack());
    }

    IEnumerator ExecuteAttack()
    {
        yield return null;
        if (turn.target == null)
        {
            owner.ChangeState<ActionState>();
            yield break;
        }
        turn.target.Damage(turn.actor.attack);
        turn.hasUnitActed = true;
        if (turn.hasUnitMoved)
            owner.ChangeState<SelectTargetState>();
        else
        {
            owner.ChangeState<ActionState>();
        }
    }


}
