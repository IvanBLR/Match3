using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameFieldSettings : MonoBehaviour
{
    public Action GameSettingsAccepted;
    public Action TryingActivateButton; // TODO: add method in AdvertisementManager, and += it in GameManager


    [SerializeField] private Image _China;
    [SerializeField] private Image _Japan;
    [SerializeField] private Image _USA;
    [SerializeField] private Image _GB;
    [SerializeField] private Image _Germany;
    [SerializeField] private Image _premium;

    private Coroutine _currentSetActiveAttention;


    [UsedImplicitly]
    public void AcceptSettings() // назначен на кнопку "Старт"
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
        if (_Japan.color.a != 1)
        {
            PrefsManager.SaveDataInt(PlayingSettingsConstant.BUTTON_FOR_ACTIVATION, 1);
            TryingActivateButton?.Invoke();
        }
        else
        {
            PrefsManager.SaveDataInt(PlayingSettingsConstant.PLAYING_SET, 1);
        }
    }

    [UsedImplicitly]
    public void SaveChina()
    {
        if (_China.color.a != 1)
        {
            PrefsManager.SaveDataInt(PlayingSettingsConstant.BUTTON_FOR_ACTIVATION, 2);
            TryingActivateButton?.Invoke();
        }
        else
        {
            PrefsManager.SaveDataInt(PlayingSettingsConstant.PLAYING_SET, 2);
        }
    }

    [UsedImplicitly]
    public void SaveGB()
    {
        if (_GB.color.a != 1)
        {
            PrefsManager.SaveDataInt(PlayingSettingsConstant.BUTTON_FOR_ACTIVATION, 5);
            TryingActivateButton?.Invoke();
        }
        else
        {
            PrefsManager.SaveDataInt(PlayingSettingsConstant.PLAYING_SET, 5);
        }
    }

    [UsedImplicitly]
    public void SaveGermany()
    {
        if (_Germany.color.a != 1)
        {
            PrefsManager.SaveDataInt(PlayingSettingsConstant.BUTTON_FOR_ACTIVATION, 6);
            TryingActivateButton?.Invoke();
        }
        else
        {
            PrefsManager.SaveDataInt(PlayingSettingsConstant.PLAYING_SET, 6);
        }
    }

    [UsedImplicitly]
    public void SavePremium()
    {
        if (_premium.color.a != 1)
        {
            PrefsManager.SaveDataInt(PlayingSettingsConstant.BUTTON_FOR_ACTIVATION, 3);
            TryingActivateButton?.Invoke();
        }
        else
        {
            PrefsManager.SaveDataInt(PlayingSettingsConstant.PLAYING_SET, 3);
        }
    }

    [UsedImplicitly]
    public void SaveUSA()
    {
        if (_USA.color.a != 1)
        {
            PrefsManager.SaveDataInt(PlayingSettingsConstant.BUTTON_FOR_ACTIVATION, 4);
            TryingActivateButton?.Invoke();
        }
        else
        {
            PrefsManager.SaveDataInt(PlayingSettingsConstant.PLAYING_SET, 4);
        }
    }

    #endregion

    public void ActivateChoosenButton(int buttonsNumber)
    {
        switch (buttonsNumber)
        {
            case 1:
                ButtonActivation(_Japan);
                break;
            case 2:
                ButtonActivation(_China);
                break;
            case 3:
                ButtonActivation(_premium);
                break;
            case 4:
                ButtonActivation(_USA);
                break;
            case 5:
                ButtonActivation(_GB);
                break;
            case 6:
                ButtonActivation(_Germany);
                break;
            default:
                Debug.LogWarning("<color=red> Some ERROR </color>");
                break;
        }
    }

    private void ButtonActivation(Image image)
    {
        Color targetColor = Color.white;
        targetColor.a = 1;
        image.color = targetColor;
    }
}