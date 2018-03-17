using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class LevelCondition
{

    public string description;
    public abstract bool IsComplete(BattleController battleController);


}
