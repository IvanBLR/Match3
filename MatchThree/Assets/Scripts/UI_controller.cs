using System;
using System.Collections;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_controller : MonoBehaviour
{
    public Action<int, int> GameFieldRawSizeChanged;
    public Action<int, int> GameFieldColumnSizeChanged;
    public Action RestartGame;

    [SerializeField] private SpriteRenderer _background;
    [SerializeField] private SpriteRenderer _soundOn;
    [SerializeField] private SpriteRenderer _soundOff;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private TextMeshProUGUI _rowSizeText;
    [SerializeField] private TextMeshProUGUI _columnSizeText;
    [SerializeField] private TextMeshProUGUI _attentionText;
    [SerializeField] private Canvas _settingsCanvas;
    [SerializeField] private Canvas _restartCanvas;
    [SerializeField] private Canvas _blurayCanvas;
    [SerializeField] private Canvas _additionalCanvas;
    [SerializeField] private Canvas _advCanvas;

    private Coroutine _currentSetActiveAttention;
    private int _previousIndex;
    private int _rowSize = 8;
    private int _columnSize = 7;
    private bool _isSoundOn = true;

    
    
    #region public methods setup in Unity Editor. Responsibility: change gameField sizes

    [UsedImplicitly]
    public void IncreaseRowSize() // назначен на кнопку + в SettingsCanvas
    {
        if (_rowSize < 12) // it was 5
            _rowSize++;
        else
        {
            if (_currentSetActiveAttention != null)
            {
                StopCoroutine(_currentSetActiveAttention);
                _attentionText.gameObject.SetActive(false);
            }

            _currentSetActiveAttention = StartCoroutine(ActivateAttentionText());
        }

        _rowSizeText.text = _rowSize.ToString();
        PrefsManager.SaveDataInt(PlayingSettingsConstant.GAME_FIELD_ROW, _rowSize);
        GameFieldRawSizeChanged?.Invoke(_rowSize, _columnSize);
    }

    [UsedImplicitly]
    public void DecreaseRowSize() // назначен на кнопку - в SettingsCanvas
    {
        if (_rowSize > 6)
            _rowSize--;
        else
        {
            if (_currentSetActiveAttention != null)
            {
                StopCoroutine(_currentSetActiveAttention);
                _attentionText.gameObject.SetActive(false);
            }

            _currentSetActiveAttention = StartCoroutine(ActivateAttentionText());
        }

        _rowSizeText.text = _rowSize.ToString();
        PrefsManager.SaveDataInt(PlayingSettingsConstant.GAME_FIELD_ROW, _rowSize);
        GameFieldRawSizeChanged?.Invoke(_rowSize, _columnSize);
    }

    [UsedImplicitly]
    public void IncreaseColumnSize() // назначен на кнопку "стрелка вверх" в SettingsCanvas
    {
        if (_columnSize < 7) // it was 8
            _columnSize++;
        else
        {
            if (_currentSetActiveAttention != null)
            {
                StopCoroutine(_currentSetActiveAttention);
                _attentionText.gameObject.SetActive(false);
            }

            _currentSetActiveAttention = StartCoroutine(ActivateAttentionText());
        }

        _columnSizeText.text = _columnSize.ToString();
        PrefsManager.SaveDataInt(PlayingSettingsConstant.GAME_FIELD_COLUMN, _columnSize);
        GameFieldColumnSizeChanged?.Invoke(_rowSize, _columnSize);
    }

    [UsedImplicitly]
    public void DecreaseColumnSize() // назначен на кнопку "стрелка вниз" в SettingsCanvas
    {
        if (_columnSize > 5)
            _columnSize--;
        else
        {
            if (_currentSetActiveAttention != null)
            {
                StopCoroutine(_currentSetActiveAttention);
                _attentionText.gameObject.SetActive(false);
            }

            _currentSetActiveAttention = StartCoroutine(ActivateAttentionText());
        }

        _columnSizeText.text = _columnSize.ToString();
        PrefsManager.SaveDataInt(PlayingSettingsConstant.GAME_FIELD_COLUMN, _columnSize);
        GameFieldColumnSizeChanged?.Invoke(_rowSize, _columnSize);
    }

    #endregion

    [UsedImplicitly]
    public void BackToMainMenu() // назначен на кнопку Рестарт в AdditionalCanvas
    {
        _restartCanvas.gameObject.SetActive(true);
        _blurayCanvas.gameObject.SetActive(true);
    }

    [UsedImplicitly]
    public void AcceptRestartProposition() // назначен на кнопку '+' , которая выскакивает после нажатия на Рестарт
    {
        RestartGame?.Invoke();
        _restartCanvas.gameObject.SetActive(false);
        _blurayCanvas.gameObject.SetActive(false);
        _additionalCanvas.gameObject.SetActive(false);
        _settingsCanvas.gameObject.SetActive(true);
    }

    [UsedImplicitly]
    public void RefuseProposition() // назначен на кнопки '-' 
    {
        _restartCanvas.gameObject.SetActive(false);
        _advCanvas.gameObject.SetActive(false);
        _blurayCanvas.gameObject.SetActive(false);
    }

    [UsedImplicitly]
    public void ChangeVolume(float volumeValue) => _audioSource.volume = volumeValue; // назначен на слайдер звука

    [UsedImplicitly]
    public void SoundTumbler() // назначен на тоггл со значком звука
    {
        _audioSource.mute = _isSoundOn;
        _soundOff.gameObject.SetActive(_isSoundOn);
        _soundOn.gameObject.SetActive(!_isSoundOn);
        _isSoundOn = !_isSoundOn;
    }

    public void LowDownBackgroundAlpha()
    {
        var color = _background.color;
        color.a = 0.5f;
        _background.color = color;
    }

    public void StartedGame()
    {
        _settingsCanvas.gameObject.SetActive(false);
        _additionalCanvas.gameObject.SetActive(true);
    }

    public void ActivateAdvCanvas(Button button)
    {
        _blurayCanvas.gameObject.SetActive(true);
        _advCanvas.gameObject.SetActive(true);
    }

    private void Awake()
    {
        PrefsManager.SaveDataInt(PlayingSettingsConstant.GAME_FIELD_ROW, _rowSize);
        PrefsManager.SaveDataInt(PlayingSettingsConstant.GAME_FIELD_COLUMN, _columnSize);
    }

    private IEnumerator ActivateAttentionText()
    {
        _attentionText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.9f);
        _attentionText.gameObject.SetActive(false);
    }
}