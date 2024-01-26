using JetBrains.Annotations;
using System;
using UnityEngine;

public class GameFieldSettings : MonoBehaviour
{
    public Action GameSettingsAccepted;

    [UsedImplicitly]
    public void AcceptSettings() // назначен на кнопку "Старт"
    {
        GameSettingsAccepted?.Invoke();
    }

    #region public methods save playing sets, setup in Unity Editor

    [UsedImplicitly]
    public void SaveBlend(int setNumber)
    {
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, setNumber);
        PlayerPrefs.Save();
    }

    [UsedImplicitly]
    public void SaveJapan(int setNumber)
    {
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, setNumber);
        PlayerPrefs.Save();
    }

    [UsedImplicitly]
    public void SaveChina(int setNumber)
    {
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, setNumber);
        PlayerPrefs.Save();
    }

    [UsedImplicitly]
    public void SaveGB(int setNumber)
    {
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, setNumber);
        PlayerPrefs.Save();
    }

    [UsedImplicitly]
    public void SaveGermany(int setNumber)
    {
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, setNumber);
        PlayerPrefs.Save();
    }

    [UsedImplicitly]
    public void SavePremium(int setNumber)
    {
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, setNumber);
        PlayerPrefs.Save();
    }

    [UsedImplicitly]
    public void SaveUSA(int setNumber)
    {
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, setNumber);
        PlayerPrefs.Save();
    }

    #endregion
}