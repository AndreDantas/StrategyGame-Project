using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    public int levelIndex;
    public bool completed;
    public bool locked;
    public List<int> unlockLevelIndex;

}
