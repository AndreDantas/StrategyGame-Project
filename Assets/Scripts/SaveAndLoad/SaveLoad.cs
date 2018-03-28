using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// Save and load class.
/// </summary>
public static class SaveLoad
{
    public static List<GameData> levelData = new List<GameData>();


    public static void Save()
    {

        levelData.Add(GameData.instance);
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Create(Path.Combine(Application.persistentDataPath, "savedGames.gd"));
        bf.Serialize(file, levelData);
        file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Path.Combine(Application.persistentDataPath, "savedGames.gd")))
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream file = File.Open(Path.Combine(Application.persistentDataPath, "savedGames.gd"), FileMode.Open);
            levelData = (List<GameData>)bf.Deserialize(file);
            file.Close();
        }
    }
}
