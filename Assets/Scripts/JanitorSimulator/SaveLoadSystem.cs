using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class SaveLoadSystem
{
    public static GameSave GameSave { get; private set; }
    public static string SaveFilePath { get; } = Application.persistentDataPath + "/save.file";

    public static void Save()
    {
        var formatter = new BinaryFormatter();
        var stream = new FileStream(SaveFilePath, FileMode.Create);

        formatter.Serialize(stream, GameSave);
        stream.Close();
    }

    public static void Load()
    {
        try
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(SaveFilePath, FileMode.Open);

            GameSave = (GameSave)formatter.Deserialize(stream);
            stream.Close();
        } 
        catch
        {
            Debug.LogWarning("No valid save file found. Proceeding with emtpy save.");
            GameSave = new GameSave();
        }
    }
}

[Serializable]
public class GameSave
{
    public int money = 0;
    public int shifts = 0;
    public int[] unlockedGadgets = new int[0];
}
