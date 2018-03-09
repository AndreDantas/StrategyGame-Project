using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class EnemyAttackState : BattleState
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
            owner.ChangeState<PlayerState>();
            yield break;
        }

        yield return turn.actor.AttackTarget(turn.target);
        if (turn.target.IsDown())
        {
            owner.RemoveKnockedDown(turn.target);
            turn.target = null;
        }
        else if (turn.target.CanCounter(turn.actor))
        {
            yield return turn.target.AttackTarget(turn.actor);
            if (turn.actor.IsDown())
            {
                owner.RemoveKnockedDown(turn.actor);
                turn.actor = null;
            }
        }

        turn.hasUnitActed = true;
        turn.target = null;
        owner.ChangeState<PlayerState>();

    }




}
