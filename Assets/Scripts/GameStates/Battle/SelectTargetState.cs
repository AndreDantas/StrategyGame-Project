using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectTargetState : BattleState
{

    public override void Enter()
    {
        base.Enter();
        StartCoroutine("ChangeCurrentUnit");
    }
    IEnumerator ChangeCurrentUnit()
    {

        if (activeUnits.Count == 0)
            yield break;

        if (turn.turnIndex > activeUnits.Count)
            turn.turnIndex = 0;

        turn.turnIndex = (turn.turnIndex + 1) % activeUnits.Count;

        if (turn.turnIndex < 0)
            turn.turnIndex = 0;



        //print("Character turn: " + index);
        turn.Change(activeUnits[turn.turnIndex]);
        yield return null;
        DeactivateSelectNode();

        if (fieldInfoController != null)
        {
            fieldInfoController.HideFieldInfoBox();
        }
        if (turn.actor.GetComponent<AIController>())
            owner.ChangeState<AIState>();
        else
            owner.ChangeState<PlayerState>();
    }
}
