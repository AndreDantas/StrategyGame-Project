using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToAttackState : BattleState
{

    public override void Enter()
    {
        base.Enter();
        StartCoroutine(Movement());
        DeactivateSelectNode();
    }

    IEnumerator Movement()
    {
        yield return null;
        List<Node> path = turn.actor.PathFind(currentNode);
        float pathCost = turn.actor.GetPathCost(path);

        if (pathCost > turn.actor.currentStamina)
        {
            owner.ChangeState<ActionState>();
            yield break;
        }

        turn.actor.WalkPath(path);
        while (turn.actor.IsMoving())
            yield return null;
        turn.actor.currentStamina -= pathCost;
        turn.hasUnitMoved = true;
        owner.ChangeState<AttackState>();
    }
}
