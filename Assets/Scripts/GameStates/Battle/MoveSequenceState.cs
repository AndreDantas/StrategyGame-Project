using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSequenceState : BattleState
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
        currentCharacter.WalkPath(currentCharacter.PathFind(currentNode));
        while (currentCharacter.IsMoving())
            yield return null;
        owner.ChangeState<SelectTargetState>();
    }
}
