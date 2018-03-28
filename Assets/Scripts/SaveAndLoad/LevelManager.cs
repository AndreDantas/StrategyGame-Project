using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    // Fix Save and loading

    public static LevelManager instance;
    public static int OverworldLevelIndex = 0;
    private static int currentLevelIndex;
    public GameData gameData;
    static bool toUnlock = false;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnLevelLoad;
        Init();
        LoadData();

    }

    void Init()
    {


        gameData = new GameData(); // Create new one
        LevelUIController[] list = FindObjectsOfType<LevelUIController>(); // Get every LevelUIController on scene
        foreach (LevelUIController l in list) //Fill data 
        {
            Level temp = l.ToLevel(); // Convert to Level format
            temp.unlockLevelIndex = new List<int>();
            foreach (LevelUIController unlock in l.unlockLevels)
                temp.unlockLevelIndex.Add(unlock.levelIndex);

            gameData.levels.Add(temp);
        }

        GameData.instance = gameData;
    }

    public void LoadData()
    {
        SaveLoad.Load();
        if (SaveLoad.levelData != null ? SaveLoad.levelData.Count > 0 : false)
        {
            GameData temp = SaveLoad.levelData[0];
            gameData.playerGold = temp.playerGold;
            gameData.playerExperience = temp.playerExperience;
            foreach (Level level in temp.levels)
            {
                Level l = null;
                l = gameData.levels.Find(x => x.levelIndex == level.levelIndex);
                if (l != null)
                {
                    l = level;
                }
            }
            UpdateMap();
        }
    }

    public void UpdateMap()
    {
        if (SceneManager.GetActiveScene().buildIndex != OverworldLevelIndex)
            return;
        if (gameData == null ? true : gameData.levels == null)
        {
            print("Init");
            Init();
            return;
        }
        List<LevelUIController> list = MathOperations.ToList(FindObjectsOfType<LevelUIController>());
        if (list == null ? true : list.Count == 0)
            return;
        foreach (Level l in gameData.levels) // For each level in the data, update the map
        {
            LevelUIController temp = null;
            temp = list.Find(x => x.levelIndex == l.levelIndex);
            if (temp)
            {
                temp.SetLevel(l);
            }
        }
    }


    private void OnLevelLoad(Scene scene, LoadSceneMode loadScene)
    {

        if (toUnlock && SceneManager.GetActiveScene().buildIndex == OverworldLevelIndex)
        {
            // Unlock Animation
            toUnlock = false;
        }
        else
        {

        }
        UpdateMap();
    }

    public void ToUnlockLevel()
    {
        toUnlock = true;
    }

    public void CompleteCurrentLevel()
    {
        if (currentLevelIndex <= -1)
            return;
        foreach (Level temp in gameData.levels)
        {
            if (temp.levelIndex == currentLevelIndex && !temp.completed && !temp.locked)
            {
                temp.completed = true;
                for (int i = 0; i < temp.unlockLevelIndex.Count; i++)
                {
                    Level unlock = null;
                    unlock = gameData.levels.Find(x => x.levelIndex == temp.unlockLevelIndex[i]);
                    if (unlock != null)
                    {
                        unlock.locked = false;
                    }
                }
                break;
            }
        }

    }

    public static void LoadLevel(Level l)
    {
        if (l == null)
            return;
        if (l.locked)
            return;

        if (Application.CanStreamedLevelBeLoaded(l.levelIndex))
            SceneManager.LoadScene(l.levelIndex);
        else
            return;
        currentLevelIndex = l.levelIndex;

    }

    public static void LoadOverworld()
    {
        if (Application.CanStreamedLevelBeLoaded(OverworldLevelIndex))
            SceneManager.LoadScene(OverworldLevelIndex);
        if (!toUnlock)
            currentLevelIndex = -1;
    }

    //public void SetLevels()
    //{
    //    levels.Clear();
    //    LevelUIController[] list = FindObjectsOfType<LevelUIController>();
    //    foreach (LevelUIController l in list)
    //    {
    //        Level temp = l.ToLevel();
    //        levels.Add(temp);
    //    }
    //}



    //public void ToCompleteLevel()
    //{
    //    levelComplete = true;
    //}

    //public void SaveLevelStatus()
    //{

    //    GameData.instance.levels = levels;
    //    SaveLoad.Save();
    //}

    //public void LoadLevelStatus()
    //{
    //    SaveLoad.Load();
    //    if (SaveLoad.levelData != null)
    //        gameData = SaveLoad.levelData[0];
    //    else
    //        gameData = new GameData();
    //    GameData.instance = gameData;
    //    if (levels == null)
    //        return;
    //    foreach (Level l in GameData.instance.levels)
    //    {
    //        Level temp = levels.Find(x => x.levelIndex == l.levelIndex);
    //        if (temp != null)
    //        {
    //            temp = l;
    //        }
    //    }

    //}



    //TEST
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            ResetProgress();
    }
    public void ResetProgress()
    {

        gameData = new GameData();
        GameData.instance = gameData;
        SaveLoad.levelData.Clear();
        SaveLoad.Save();
        LoadOverworld();
    }
}
