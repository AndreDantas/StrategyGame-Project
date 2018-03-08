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

        yield return AttackTarget(turn.actor, turn.target);
        if (turn.target.IsDown())
        {
            RemoveKnockedDown(turn.target);
            turn.target = null;
        }
        else if (turn.target.CanCounter(turn.actor))
        {
            yield return AttackTarget(turn.target, turn.actor);
            if (turn.actor.IsDown())
            {
                RemoveKnockedDown(turn.actor);
                turn.actor = null;
            }
        }

        turn.hasUnitActed = true;
        turn.target = null;
        if (turn.hasUnitMoved || turn.actor == null)
            owner.ChangeState<SelectTargetState>();
        else
        {
            owner.ChangeState<ActionState>();
        }
    }

    static IEnumerator AttackTarget(Character attacker, Character target)
    {

        attacker.AttackAnim(target.x, target.y); // Start attack animation.

        while (attacker.IsAttacking())
            yield return null;

        yield return null;

        target.DamageAnim(attacker.Attack()); // Start damage animation.

        while (target.IsTakingDamage())
            yield return null;

        target.Damage(attacker.Attack()); // Deal damage to target.

    }

    public void RemoveKnockedDown(Character down)
    {
        if (down.IsDown() && activeUnits != null ? activeUnits.Contains(down) : false)
        {
            if (activeUnits.IndexOf(down) <= turn.turnIndex)
                turn.turnIndex--;
            down.RemoveFromMap();
            activeUnits.Remove(down);
            knockedDownUnits.Add(down);
            down.gameObject.SetActive(false);
        }
    }

}
