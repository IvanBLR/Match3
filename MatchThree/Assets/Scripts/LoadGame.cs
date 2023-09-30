using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGame : MonoBehaviour
{
    [SerializeField] private Canvas _invalidCanvas;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _soundOn;
    [SerializeField] private Image _soundOff;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _bottonClick;

    private bool _isSoundOn = true;

    [UsedImplicitly]
    public void StartGame()
    {
        _audioSource.PlayOneShot(_bottonClick);
        int sound = _isSoundOn ? 1 : 0;
        PlayerPrefs.SetInt(PlayingSettingsConstant.SOUND_ON_OFF, sound);
        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }

    [UsedImplicitly]
    public void SoundTumbler()
    {
        _audioSource.mute = _isSoundOn;
        _soundOff.gameObject.SetActive(_isSoundOn);
        _soundOn.gameObject.SetActive(!_isSoundOn);
        _isSoundOn = !_isSoundOn;
    }

    [UsedImplicitly]
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    private void Awake()
    {
        //StartCoroutine(CheckValidateWindowAspect());
    }

    private IEnumerator CheckValidateWindowAspect()
    {
        double width = Screen.width;
        double height = Screen.height;
        double currentSize = Math.Round((width / height), 2);
        
        double expectedWidth = PlayingSettingsConstant.SCREEN_ROW;
        double expectedHeight = PlayingSettingsConstant.SCREEN_COLUMN;
        double expectedSize = Math.Round((expectedWidth / expectedHeight), 2);
;
        if (!(currentSize == expectedSize))
        {
            _invalidCanvas.gameObject.SetActive(true);
            _canvas.gameObject.SetActive(false);
            _audioSource.gameObject.SetActive(false);
            transform.GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            _invalidCanvas.gameObject.SetActive(false);
            _canvas.gameObject.SetActive(true);
            _audioSource.gameObject.SetActive(true);
        }

        yield return new WaitUntil(() => currentSize == expectedSize);
    }
}