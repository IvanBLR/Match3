using UnityEngine;

public static class PrefsManager
{
    public static void SaveDataInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    public static int GetDataInt(string key) => PlayerPrefs.GetInt(key);
}