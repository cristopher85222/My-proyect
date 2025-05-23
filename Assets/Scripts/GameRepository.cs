using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameRepository
{
    public static GameData GetData()
    {
        string path = Application.persistentDataPath + "/gameData.save";

        if (!File.Exists(path))
            return new GameData();

        FileStream fs = File.OpenRead(path);
        BinaryFormatter bf = new BinaryFormatter();
        GameData data = (GameData)bf.Deserialize(fs);
        fs.Close();

        return data;
    }

    public static void SaveData(GameData data)
    {
        string path = Application.persistentDataPath + "/gameData.save";

        FileStream fs;
        if (File.Exists(path))
        {
            fs = File.OpenWrite(path);
        }
        else
        {
            fs = File.Create(path);
        }

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, data);
        fs.Close();
    }
}
