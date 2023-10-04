using System;
using JetBrains.Annotations;
using UnityEngine;

public class AdvertisementManager : MonoBehaviour
{
    public Action<int> ActivateAutoButton;
    public Action CloseAdvCanvas;

    [UsedImplicitly]
    public void ActivateAdvertisement() // будет назначена на "+" в AdvertisementCanvas
    {
        int activatedButtonNumber = PrefsManager.GetDataInt(PlayingSettingsConstant.BUTTON_FOR_ACTIVATION);
        ActivateAutoButton?.Invoke(activatedButtonNumber);
        PrefsManager.SaveDataInt(PlayingSettingsConstant.PLAYING_SET, activatedButtonNumber);
        
        CloseAdvCanvas?.Invoke();
        /*
         * TODO: add advertisement activation
         */
        
        
        
    }
}