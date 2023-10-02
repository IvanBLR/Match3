using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameFieldSettings : MonoBehaviour
{
    public Action GameSettingsAccepted;
   // public Action<int> ActivatedEasyLevel;
   // public Action<int> ActivatedMiddleLevel;
   // public Action<int> ActivatedHardLevel;

    [SerializeField] private Button _blend;
    [SerializeField] private Button _China;
    [SerializeField] private Button _Japan;
    [SerializeField] private Button _USA;
    [SerializeField] private Button _GB;
    [SerializeField] private Button _Germany;
    [SerializeField] private Button _premium;

    private Coroutine _currentSetActiveAttention;

   // [UsedImplicitly]
   // public void RestartGame()
   // {
   //     SceneManager.LoadScene(0);
   // }

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
        //ActivatedEasyLevel?.Invoke(0);
    }

    [UsedImplicitly]
    public void SaveJapan()
    {
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, 1);
        PlayerPrefs.Save();
        //ActivatedMiddleLevel?.Invoke(1);
    }

    [UsedImplicitly]
    public void SaveChina()
    {
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, 2);
        PlayerPrefs.Save();
       // ActivatedHardLevel?.Invoke(2);
    }

    [UsedImplicitly]
    public void SaveGB()
    {
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, 5);
        PlayerPrefs.Save();
    }

    [UsedImplicitly]
    public void SaveGermany()
    {
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, 6);
        PlayerPrefs.Save();
    }

    [UsedImplicitly]
    public void SavePremium()
    {
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, 3);
        PlayerPrefs.Save();
    }

    [UsedImplicitly]
    public void SaveUSA()
    {
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, 4);
        PlayerPrefs.Save();
    }
    #endregion
}