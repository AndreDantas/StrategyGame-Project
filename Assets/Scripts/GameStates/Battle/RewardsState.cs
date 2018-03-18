using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardsState : BattleState
{
    LevelRewardManager rewardManager;
    protected override void Awake()
    {
        base.Awake();
        rewardManager = LevelRewardManager.instance;
    }
    public override void Enter()
    {
        base.Enter();
        if (rewardManager)
            rewardManager.OpenWindow();
        else
        {
            // Continue to level select.
        }
    }

    protected override void AddListeners()
    {
        base.AddListeners();
        if (rewardManager)
        {
            rewardManager.OnContinue += OnContinue;
        }
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        if (rewardManager)
        {
            rewardManager.OnContinue -= OnContinue;
        }
    }

    public void OnContinue()
    {
        if (rewardManager)
            rewardManager.CloseWindow();

        LevelManager.LoadLevel(0);
        // Continue to level select.
    }
}
