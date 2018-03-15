using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelDetails : MonoBehaviour
{

    public List<LevelCondition> winConditions;
    public List<LevelCondition> failConditions;

    protected virtual void Start()
    {
        winConditions = new List<LevelCondition>();
        failConditions = new List<LevelCondition>();
    }
}
