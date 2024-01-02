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
    private int _x;

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
        if (PrefsManager.GetDataInt(PlayingSettingsConstant.JAPAN) != 1)
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
        if (PrefsManager.GetDataInt(PlayingSettingsConstant.CHINA) != 1)
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
        if (PrefsManager.GetDataInt(PlayingSettingsConstant.GB) != 1)
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
        if (PrefsManager.GetDataInt(PlayingSettingsConstant.GERMANY) != 1)
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
        if (PrefsManager.GetDataInt(PlayingSettingsConstant.PREMIUM) != 1)
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
        if (PrefsManager.GetDataInt(PlayingSettingsConstant.USA) != 1)
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

    public void XXXXX(int x)
    {
        _x = x;
    }
    public void ActivateChoosenButton()
    {
        switch (_x)
        {
            case 1:
                ButtonActivation(_Japan);
                PrefsManager.SaveDataInt(PlayingSettingsConstant.JAPAN, 1);
                break;
            case 2:
                ButtonActivation(_China);
                PrefsManager.SaveDataInt(PlayingSettingsConstant.CHINA, 1);
                break;
            case 3:
                ButtonActivation(_premium);
                PrefsManager.SaveDataInt(PlayingSettingsConstant.PREMIUM, 1);
                break;
            case 4:
                ButtonActivation(_USA);
                PrefsManager.SaveDataInt(PlayingSettingsConstant.USA, 1);
                break;
            case 5:
                ButtonActivation(_GB);
                PrefsManager.SaveDataInt(PlayingSettingsConstant.GB, 1);
                break;
            case 6:
                ButtonActivation(_Germany);
                PrefsManager.SaveDataInt(PlayingSettingsConstant.GERMANY, 1);
                break;
            default:
                Debug.LogWarning("<color=red> Some ERROR </color>");
                break;
        }
    }

    private void Start()
    {
        if (PrefsManager.GetDataInt(PlayingSettingsConstant.USA) == 1)
            ButtonActivation(_USA);
        if (PrefsManager.GetDataInt(PlayingSettingsConstant.JAPAN) == 1)
            ButtonActivation(_Japan);
        if (PrefsManager.GetDataInt(PlayingSettingsConstant.GB) == 1)
            ButtonActivation(_GB);
        if (PrefsManager.GetDataInt(PlayingSettingsConstant.CHINA) == 1)
            ButtonActivation(_China);
        if (PrefsManager.GetDataInt(PlayingSettingsConstant.GERMANY) == 1)
            ButtonActivation(_Germany);
        if (PrefsManager.GetDataInt(PlayingSettingsConstant.PREMIUM) == 1)
            ButtonActivation(_premium);
    }

    private void ButtonActivation(Image image)
    {
        Color targetColor = Color.white;
        targetColor.a = 1;
        image.color = targetColor;
    }
}