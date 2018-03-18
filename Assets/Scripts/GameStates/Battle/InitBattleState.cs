using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitBattleState : BattleState
{

    public override void Enter()
    {
        base.Enter();
        StartCoroutine(Init());
    }
    IEnumerator Init()
    {
        //Actions before battle.
        HideFieldInfoBox();
        turn.turnCount = 0;
        turn.turnIndex = -1;
        for (int i = activeUnits.Count - 1; i >= 0; i--)
        {

            if (activeUnits[i].gameObject.activeSelf == false)
                activeUnits.Remove(activeUnits[i]);

        }
        yield return null;
        owner.ChangeState<SelectTargetState>();
    }
}
