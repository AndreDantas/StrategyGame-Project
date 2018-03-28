using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[System.Serializable]
public class GameData
{
    public static GameData instance;
    public int playerGold;
    public int playerExperience;
    public List<Level> levels = new List<Level>();



}
