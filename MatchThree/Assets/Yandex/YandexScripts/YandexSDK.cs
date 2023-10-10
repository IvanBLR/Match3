using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEngine;

public class YandexSDK : MonoBehaviour
{
    public Action AdvertisementStarted;
    public Action AdvertisementFinished;

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

    [UsedImplicitly]
    public void AuthenticateSuccessed(string data) // вызывается из index.html при успешной авторизации пользователя
    {
        PlayerPrefs.SetString(PlayingSettingsConstant.USER_NAME, data);
        PlayerPrefs.Save();
    }

    [UsedImplicitly]
    public void SoundOn() // use in index.html
    {
        AdvertisementFinished?.Invoke();
    }

    public void Authenticate()
    {
        Auth();
    }

    public void ShowCommonAdvertisement()
    {
        AdvertisementStarted?.Invoke();
        ShowAdv();
    }


    public void ShowRewardAdvertisement()
    {
        AdvertisementStarted?.Invoke();
        ShowRewardAdv();
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}