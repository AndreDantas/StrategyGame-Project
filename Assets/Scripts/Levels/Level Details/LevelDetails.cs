using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelState
{
    Victory,
    Loss,
    InProgress

}
public abstract class LevelDetails : MonoBehaviour
{

    public BattleController battleController;
    [ReadOnly]
    [SerializeField]
    LevelState _levelState;
    public LevelState levelState
    {
        get
        {
            CheckConditions();
            return _levelState;
        }

        internal set { _levelState = value; }
    }
    public List<LevelCondition> winConditions;
    public List<LevelCondition> failConditions;

    protected virtual void Start()
    {
        winConditions = new List<LevelCondition>();
        failConditions = new List<LevelCondition>();
    }

    public virtual void CheckConditions()
    {
        if (AnyConditionMet(failConditions, battleController))
        {
            _levelState = LevelState.Loss;
            return;

        }
        if (AllConditionsMet(winConditions, battleController))
        {
            _levelState = LevelState.Victory;
            return;
        }

        _levelState = LevelState.InProgress;
    }

    public static bool AllConditionsMet(List<LevelCondition> conditions, BattleController battleController)
    {
        bool result = true;
        foreach (LevelCondition l in conditions)
        {
            if (!l.IsComplete(battleController))
            {
                result = false;
                break;
            }
        }
        return result;
    }

    public static bool AnyConditionMet(List<LevelCondition> conditions, BattleController battleController)
    {
        bool result = false;

        foreach (LevelCondition l in conditions)
        {
            if (l.IsComplete(battleController))
            {
                result = true;
                break;
            }
        }
        return result;
    }

    public virtual void SetLevelRewards()
    {
        LevelRewardManager.instance.AddReward("gold", 100);
    }
}
