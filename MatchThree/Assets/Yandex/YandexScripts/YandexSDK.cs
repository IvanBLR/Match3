using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEngine;

public class YandexSDK : MonoBehaviour
{
    public static YandexSDK Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<YandexSDK>();
            }

            return _instance;
        }
    }

    private static YandexSDK _instance;

    [DllImport("__Internal")]
    private static extern void Auth();

    [DllImport("__Internal")]
    private static extern void ShowAdv();

    [DllImport("__Internal")]
    private static extern void ShowRewardAdv();

    public void RewardVideoStarted() {}

    public void RewardVideoClosed() {}

    public void Authenticate()
    {
        Auth();
    }


    public void ShowCommonAdvertisement()
    {
        ShowAdv();
    }

    public void ShowRewardAdvertisement()
    {
        ShowRewardAdv();
    }

    [UsedImplicitly]
    public void AuthenticateSuccessed(string data) // вызывается из index.html при успешной авторизации пользователя
    {
        PlayerPrefs.SetString(PlayingSettingsConstant.USER_NAME, data);
        PlayerPrefs.Save();
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}