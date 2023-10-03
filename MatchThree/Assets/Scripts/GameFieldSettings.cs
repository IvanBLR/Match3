using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameFieldSettings : MonoBehaviour
{
    public Action GameSettingsAccepted;
    public Action<Button> UnblocedButtonSet;// TODO: add method in AdvertisementManager, and += it in GameManager

    [SerializeField] private Button _blend;
    [SerializeField] private Button _China;
    [SerializeField] private Button _Japan;
    [SerializeField] private Button _USA;
    [SerializeField] private Button _GB;
    [SerializeField] private Button _Germany;
    [SerializeField] private Button _premium;

    private Coroutine _currentSetActiveAttention;


    [UsedImplicitly]
    public void AcceptSettings()
    {
        GameSettingsAccepted?.Invoke();
    }

    #region public methods save playing sets, setup in Unity Editor

    [UsedImplicitly]
    public void SaveBlend()
    {
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, 0);
        PlayerPrefs.Save();
    }

    [UsedImplicitly]
    public void SaveJapan()
    {
        UnblocedButtonSet?.Invoke(_Japan);
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, 1);
        PlayerPrefs.Save();
    }

    [UsedImplicitly]
    public void SaveChina()
    {
        UnblocedButtonSet?.Invoke(_China);
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, 2);
        PlayerPrefs.Save();
    }

    [UsedImplicitly]
    public void SaveGB()
    {
        UnblocedButtonSet?.Invoke(_GB);
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, 5);
        PlayerPrefs.Save();
    }

    [UsedImplicitly]
    public void SaveGermany()
    {
        UnblocedButtonSet?.Invoke(_Germany);
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, 6);
        PlayerPrefs.Save();
    }

    [UsedImplicitly]
    public void SavePremium()
    {
        UnblocedButtonSet?.Invoke(_premium);
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, 3);
        PlayerPrefs.Save();
    }

    [UsedImplicitly]
    public void SaveUSA()
    {
        UnblocedButtonSet?.Invoke(_USA);
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, 4);
        PlayerPrefs.Save();
    }

    #endregion
}