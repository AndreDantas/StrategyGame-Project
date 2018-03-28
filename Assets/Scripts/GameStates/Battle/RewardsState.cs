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
        foreach (LevelReward l in rewardManager.GetRewards())
            l.GetReward();

        if (LevelManager.instance)
        {
            LevelManager.instance.CompleteCurrentLevel();
            SaveLoad.Save();

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

        owner.ChangeState<ReturnLevelScreenState>();
        // Continue to level select.
    }
}
