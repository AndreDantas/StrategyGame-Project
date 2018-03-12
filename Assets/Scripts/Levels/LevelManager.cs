using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public List<Level> levels;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }


    public void LoadLevel(Level l)
    {
        if (l == null ? true : l.locked)
            return;
        SceneManager.LoadScene(l.levelIndex);

    }
}
