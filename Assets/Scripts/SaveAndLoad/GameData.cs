using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[System.Serializable]
public class GameData
{
    public static GameData instance;
    [SerializeField]
    protected int playerGold;
    [SerializeField]
    protected int playerExperience;
    public List<LevelUI> levels = new List<LevelUI>();


    public void LoadLevel(LevelUI l)
    {
        if (l == null ? true : l.locked)
            return;
        SceneManager.LoadScene(l.levelIndex);

    }

    public int GetGold()
    {
        return playerGold;
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

    public int GetExperience()
    {
        return playerExperience;
    }

    public void AddExperience(int add)
    {
        add = MathOperations.ClampMin(add, 0);
        playerExperience += add;
    }

}
