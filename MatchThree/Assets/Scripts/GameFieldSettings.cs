using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFieldSettings : MonoBehaviour
{
    public Action GameSettingsAccepted;
    public Action<int> ActivatedEasyLevel;
    public Action<int> ActivatedMiddleLevel;
    public Action<int> ActivatedHardLevel;

    [SerializeField] private Button _autoSet;
    [SerializeField] private Button _westerosSet;
    [SerializeField] private Button _forexSet;

    private Coroutine _currentSetActiveAttention;

    [UsedImplicitly]
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    [UsedImplicitly]
    public void AcceptSettings()
    {
        GameSettingsAccepted?.Invoke();
    }

    #region public methods save playing sets, setup in Unity Editor

    [UsedImplicitly]
    public void SaveAutoSet()
    {
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, 0);
        PlayerPrefs.Save();
        ActivatedEasyLevel?.Invoke(0);
    }

    [UsedImplicitly]
    public void SaveWesterosSet()
    {
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, 1);
        PlayerPrefs.Save();
        ActivatedMiddleLevel?.Invoke(1);
    }

    [UsedImplicitly]
    public void SaveForexSet()
    {
        PlayerPrefs.SetInt(PlayingSettingsConstant.PLAYING_SET, 2);
        PlayerPrefs.Save();
        ActivatedHardLevel?.Invoke(2);
    }

    #endregion
}