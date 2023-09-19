using JetBrains.Annotations;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFieldSettings : MonoBehaviour
{
    public Action GameSettingsAccepted;

    public Action<int, int> GameFieldRawSizeChanged;
    public Action<int, int> GameFieldColumnSizeChanged;

    [SerializeField] private TextMeshProUGUI _rowSizeText;
    [SerializeField] private TextMeshProUGUI _columnSizeText;
    [SerializeField] private TextMeshProUGUI _attentionText;

    [SerializeField] private SpriteRenderer _background;

    [SerializeField] private Canvas _settingsCanvas;
    [SerializeField] private Canvas _invalidCanvas;

    [SerializeField] private Button _autoSet;
    [SerializeField] private Button _westerosSet;
    [SerializeField] private Button _forexSet;

    [SerializeField] private Sprite[] _backgroundSprites = new Sprite[3];

    private int _rowSize = 4;
    private int _columnSize = 6;
    private Coroutine _currentSetActiveAttention;

    private void Awake()
    {
        _attentionText.enabled = false;
        PlayerPrefs.SetInt(PlayerSettingsConst.GAME_FIELD_ROW, _rowSize);
        PlayerPrefs.SetInt(PlayerSettingsConst.GAME_FIELD_COLUMN, _columnSize);
        PlayerPrefs.Save();
        _invalidCanvas.enabled = false;
        StartCoroutine(CheckValidateWindowAspect());
    }


    private IEnumerator CheckValidateWindowAspect()
    {
        var width = Screen.width;
        var height = Screen.height;
        var currentScreenSize = width.ToString() + "x" + height.ToString();
        if (!currentScreenSize.Equals(PlayerSettingsConst.SCREEN_SIZE))
        {
            _settingsCanvas.enabled = false;
            _invalidCanvas.enabled = true;
        }

        yield return new WaitUntil(() => PlayerSettingsConst.SCREEN_SIZE == currentScreenSize);
    }

    [UsedImplicitly]
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    #region public methods, use in ScreenSettings Menu, setup in Unity Editor. Responsibility: change gameField sizes

    [UsedImplicitly]
    public void IncreaseRowSize()
    {
        if (_rowSize < 5)
        {
            _rowSize++;
        }
        else
        {
            if (_currentSetActiveAttention != null)
            {
                StopCoroutine(_currentSetActiveAttention);
                _attentionText.enabled = false;
            }
            _currentSetActiveAttention = StartCoroutine(ActivateAttentionText());
        }

        _rowSizeText.text = _rowSize.ToString();
        PlayerPrefs.SetInt(PlayerSettingsConst.GAME_FIELD_ROW, _rowSize);
        PlayerPrefs.Save();

        GameFieldRawSizeChanged?.Invoke(_rowSize, _columnSize);
    }

    [UsedImplicitly]
    public void DecreaseRowSize()
    {
        if (_rowSize > 3)
        {
            _rowSize--;
        }
        else
        {
            if (_currentSetActiveAttention != null)
            {
                StopCoroutine(_currentSetActiveAttention);
                _attentionText.enabled = false;
            }

            _currentSetActiveAttention = StartCoroutine(ActivateAttentionText());
        }

        _rowSizeText.text = _rowSize.ToString();
        PlayerPrefs.SetInt(PlayerSettingsConst.GAME_FIELD_ROW, _rowSize);
        PlayerPrefs.Save();

        GameFieldRawSizeChanged?.Invoke(_rowSize, _columnSize);
    }

    [UsedImplicitly]
    public void IncreaseColumnSize()
    {
        if (_columnSize < 8)
        {
            _columnSize++;
        }
        else
        {
            if (_currentSetActiveAttention != null)
            {
                StopCoroutine(_currentSetActiveAttention);
                _attentionText.enabled = false;
            }

            _currentSetActiveAttention = StartCoroutine(ActivateAttentionText());
        }

        _columnSizeText.text = _columnSize.ToString();
        PlayerPrefs.SetInt(PlayerSettingsConst.GAME_FIELD_COLUMN, _columnSize);
        PlayerPrefs.Save();

        GameFieldColumnSizeChanged?.Invoke(_rowSize, _columnSize);
    }

    [UsedImplicitly]
    public void DecreaseColumnSize()
    {
        if (_columnSize > 5)
        {
            _columnSize--;
        }
        else
        {
            if (_currentSetActiveAttention != null)
            {
                StopCoroutine(_currentSetActiveAttention);
                _attentionText.enabled = false;
            }

            _currentSetActiveAttention = StartCoroutine(ActivateAttentionText());
        }

        _columnSizeText.text = _columnSize.ToString();
        PlayerPrefs.SetInt(PlayerSettingsConst.GAME_FIELD_COLUMN, _columnSize);
        PlayerPrefs.Save();

        GameFieldColumnSizeChanged?.Invoke(_rowSize, _columnSize);
    }

    #endregion


    [UsedImplicitly]
    public void AcceptSettings()
    {
        var color = _background.color;
        color.a = 0.5f;
        _background.color = color;

        _settingsCanvas.enabled = false;

        GameSettingsAccepted?.Invoke();
    }

    #region public methods, save playing sets, setup in Unity Editor

    [UsedImplicitly]
    public void SaveAutoSet()
    {
        PlayerPrefs.SetInt(PlayerSettingsConst.PLAYING_SET, 0);
        PlayerPrefs.Save();
        _background.sprite = _backgroundSprites[0];
    }

    [UsedImplicitly]
    public void SaveWesterosSet()
    {
        PlayerPrefs.SetInt(PlayerSettingsConst.PLAYING_SET, 2);
        PlayerPrefs.Save();
        _background.sprite = _backgroundSprites[1];
    }

    [UsedImplicitly]
    public void SaveForexSet()
    {
        PlayerPrefs.SetInt(PlayerSettingsConst.PLAYING_SET, 1);
        PlayerPrefs.Save();
        _background.sprite = _backgroundSprites[2];
    }

    #endregion

    private IEnumerator ActivateAttentionText()
    {
        _attentionText.enabled = true;
        yield return new WaitForSeconds(0.4f);
        _attentionText.enabled = false;
    }
}