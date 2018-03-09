using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSequenceState : BattleState
{

    public override void Enter()
    {
        base.Enter();
        StartCoroutine(Movement());
    }

    IEnumerator Movement()
    {
        yield return null;
        List<Node> path = turn.actor.PathFind(movementNode);
        float pathCost = turn.actor.GetPathCost(path);

        if (pathCost > turn.actor.currentStamina)
        {
            owner.ChangeState<PlayerState>();
            yield break;
        }

        turn.actor.WalkPath(path);
        while (turn.actor.IsMoving())
            yield return null;
        turn.actor.currentStamina -= pathCost;
        turn.hasUnitMoved = true;
        owner.ChangeState<PlayerState>();
    }
}
