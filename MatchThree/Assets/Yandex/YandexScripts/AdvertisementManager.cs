using System;
using JetBrains.Annotations;
using UnityEngine;

public class AdvertisementManager : MonoBehaviour
{
    public Action<int> ActivateAutoButton;
    public Action CloseAdvCanvas;
    public Action CloseAuthCanvas;

    [UsedImplicitly]
    public void ActivateAdvertisement() // назначен на "+" в AdvertisementCanvas
    {
        int activatedButtonNumber = PrefsManager.GetDataInt(PlayingSettingsConstant.BUTTON_FOR_ACTIVATION);
        ActivateAutoButton?.Invoke(activatedButtonNumber);
        PrefsManager.SaveDataInt(PlayingSettingsConstant.PLAYING_SET, activatedButtonNumber);
        
        CloseAdvCanvas?.Invoke();
        /*
         * TODO: add advertisement activation
         */
        
        
        
    }

    [UsedImplicitly]
    public void ActivateGameRate()// назначен на "+" в AuthorizationCanvas
    {
        CloseAuthCanvas?.Invoke();
        /*
         * TODO: call ysdk.Auth.method and after rate.method
         */
    }
}