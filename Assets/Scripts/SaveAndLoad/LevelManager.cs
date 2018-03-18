using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    [HideInInspector]
    public GameData gameData;
    public List<LevelUIController> levels = new List<LevelUIController>();
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Use this for initialization
    void Start()
    {
        //gameData = new GameData();
        GameData.instance = gameData;
        SaveLoad.Load();
    }

    public static void LoadLevel(LevelUI l)
    {
        if (l == null)
            return;
        if (l.locked)
            return;

        if (Application.CanStreamedLevelBeLoaded(l.levelIndex))
            SceneManager.LoadScene(l.levelIndex);
    }

    public static void LoadLevel(int index)
    {
        if (Application.CanStreamedLevelBeLoaded(index))
            SceneManager.LoadScene(index);
    }
}
