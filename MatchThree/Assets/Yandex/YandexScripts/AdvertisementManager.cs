using System;
using JetBrains.Annotations;
using UnityEngine;
using YG;

public class AdvertisementManager : MonoBehaviour
{
    public Action<int> ActivateAutoButton;
    public Action CloseAdvCanvas;
    public Action CloseAuthCanvas;
    public Action AdvStart;
    public Action AdvFinish;
    
    private YandexSDK _sdk;

    [UsedImplicitly]
    public void ActivateAdvertisement() // назначен на "+" в AdvertisementCanvas
    {
        int activatedButtonNumber = PrefsManager.GetDataInt(PlayingSettingsConstant.BUTTON_FOR_ACTIVATION);
        ActivateAutoButton?.Invoke(activatedButtonNumber);
        PrefsManager.SaveDataInt(PlayingSettingsConstant.PLAYING_SET, activatedButtonNumber);

        CloseAdvCanvas?.Invoke();
        _sdk.ShowRewardAdvertisement();
    }

    [UsedImplicitly]
    public void ActivateGameRate() // назначен на "+" в AuthorizationCanvas
    {
        YandexGame.ReviewShow(true);
        
        
        CloseAuthCanvas?.Invoke();
        //_sdk.Authenticate();
    }

    // public void ActivateSimpleAdvertisement()
    // {
    //     _sdk.ShowCommonAdvertisement();
    // }

    private void Start()
    {
        _sdk = YandexSDK.Instance;
        _sdk.AdvertisementStarted += AdvStarted;
        _sdk.AdvertisementFinished += AdvFinished;
    }

    private void AdvStarted()
    {
        AdvStart?.Invoke();
    }

    private void AdvFinished()
    {
        AdvFinish?.Invoke();
    }

    private void OnDestroy()
    {
        _sdk.AdvertisementStarted -= AdvStarted;
        _sdk.AdvertisementFinished -= AdvFinished;
    }
}