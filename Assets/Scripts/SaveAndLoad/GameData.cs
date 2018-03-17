using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[System.Serializable]
public class GameData
{
    public static GameData instance;
    public int playerGold { get; internal set; }
    public int playerExperience { get; internal set; }
    public List<LevelUI> levels;


    public void LoadLevel(LevelUI l)
    {
        if (l == null ? true : l.locked)
            return;
        SceneManager.LoadScene(l.levelIndex);

    }

    public void AddGold(int add)
    {
        add = MathOperations.ClampMin(add, 0);
        playerGold += add;
    }

    public bool RemoveGold(int remove)
    {
        if (remove > playerGold)
            return false;

        playerGold -= remove;
        return true;
    }

    public void AddExperience(int add)
    {
        add = MathOperations.ClampMin(add, 0);
        playerExperience += add;
    }

}
