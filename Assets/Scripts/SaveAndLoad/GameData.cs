using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[System.Serializable]
public class GameData
{
    public static GameData instance;
    public List<Level> levels;


    public void LoadLevel(Level l)
    {
        if (l == null ? true : l.locked)
            return;
        SceneManager.LoadScene(l.levelIndex);

    }
}
