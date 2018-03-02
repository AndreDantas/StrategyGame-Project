using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectTargetState : BattleState
{

    int index = -1;
    public override void Enter()
    {
        base.Enter();
        StartCoroutine("ChangeCurrentUnit");
    }
    IEnumerator ChangeCurrentUnit()
    {
        if (activeUnits.Count == 0)
            yield break;
        index = (index + 1) % activeUnits.Count;
        turn.Change(activeUnits[index]);
        yield return null;
        DeactivateSelectNode();

        if (fieldInfoController != null)
        {
            fieldInfoController.HideFieldInfoBox();
        }

        owner.ChangeState<ActionState>();
    }
}
