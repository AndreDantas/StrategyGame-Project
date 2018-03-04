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

        turn.actor.AttackAnim(turn.target.x, turn.target.y); // Start attack animation.

        while (turn.actor.IsAttacking())
            yield return null;

        yield return null;

        turn.target.DamageAnim(turn.actor.Attack()); // Start damage animation.

        while (turn.target.IsTakingDamage())
            yield return null;

        turn.target.Damage(turn.actor.Attack()); // Deal damage to target.

        if (turn.target.IsDown())
        {
            if (activeUnits.IndexOf(turn.target) <= turn.turnIndex)
                turn.turnIndex--;
            turn.target.RemoveFromMap();
            activeUnits.Remove(turn.target);
            knockedDownUnits.Add(turn.target);
            turn.target.gameObject.SetActive(false);
            turn.target = null;
        }

        turn.hasUnitActed = true;
        turn.target = null;
        if (turn.hasUnitMoved)
            owner.ChangeState<SelectTargetState>();
        else
        {
            owner.ChangeState<ActionState>();
        }
    }


}
