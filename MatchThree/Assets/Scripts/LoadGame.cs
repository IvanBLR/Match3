using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGame : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;
    [SerializeField] private Image _soundOn;
    [SerializeField] private Image _soundOff;
    [SerializeField] private AudioSource _audioSource;
    private bool _isSoundOn = true;

    [UsedImplicitly]
    public void StartGame()
    {
        int sound = _isSoundOn ? 1 : 0;
        PlayerPrefs.SetInt(SettingsConstant.SOUND_ON_OFF, sound);
        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }

    [UsedImplicitly]
    public void SoundTumbler()
    {
        _soundOff.gameObject.SetActive(_isSoundOn);
        _soundOn.gameObject.SetActive(!_isSoundOn);
        _isSoundOn = !_isSoundOn;
        _audioSource.gameObject.SetActive(_isSoundOn);
    }
}