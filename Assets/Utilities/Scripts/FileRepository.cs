using System.IO;
using UnityEngine;

public static class FileRepository
{
    public static string GetPath(string key)
    {
        if (Directory.Exists(Application.persistentDataPath))
            return Path.Combine(Application.persistentDataPath, $"{key}.txt");

        return Path.Combine(Application.streamingAssetsPath, $"{key}.txt");
    }

    public static bool HasKey(string key)
    {
        return File.Exists(GetPath(key));
    }

    public static FileStream GetFileStream(string key, FileAccess access)
    {
        return new FileStream(GetPath(key), FileMode.OpenOrCreate, access);
    }

    public static void SetJsonData<T>(T value, string key)
    {
        string path = GetPath(key);

        string jsonStr = JsonUtility.ToJson(value);

        File.WriteAllText(path, jsonStr);
    }

    public static T GetJsonData<T>(string key)
    {
        string path = GetPath(key);

        CheckFileExistence<T>(path);

        string jsonStr = File.ReadAllText(path);
        try
        {
            T result = JsonUtility.FromJson<T>(jsonStr);
            //Debug.Log("Ok");
            return result;
        }
        catch (System.ArgumentException)
        {
            //Debug.Log("Null");
            SetJsonData<T>(default, key);
            return GetJsonData<T>(key);
        }
    }

    private static bool CheckFileExistence<T>(string path)
    {
        if (File.Exists(path))
            return true;

        string jsonStr = JsonUtility.ToJson(default(T));

        File.WriteAllText(path, jsonStr);

        return false;
    }
}
