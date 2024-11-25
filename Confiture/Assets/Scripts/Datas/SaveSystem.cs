using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public static class SaveSystem
{
    public static void SaveSave(string saveName, SaveData saveData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + saveName + ".save";

        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, saveData);
        stream.Close();
    }

    public static SaveData LoadSaveData(string saveName)
    {
        string path = Application.persistentDataPath + saveName + ".save";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }

    public static void DeleteSaveData(string saveName)
    {
        string path = Application.persistentDataPath + saveName + ".save";

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
