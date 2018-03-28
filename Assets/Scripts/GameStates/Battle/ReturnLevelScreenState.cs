using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnLevelScreenState : BattleState
{

    public override void Enter()
    {
        base.Enter();
        StartCoroutine(ReturnOverworld());
    }

    IEnumerator ReturnOverworld()
    {
        yield return null;
        LevelManager.LoadOverworld();
    }
}
