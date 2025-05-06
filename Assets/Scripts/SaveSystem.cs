using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/save.json";

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Сохранено: " + savePath);
    }

    public static SaveData Load()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("Файл сохранения не найден.");
            return new SaveData();
        }

        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void Clear()
    {
        if (File.Exists(savePath))
            File.Delete(savePath);
    }
}
