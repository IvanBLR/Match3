using JetBrains.Annotations;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameFieldSettings : MonoBehaviour
{
    public Action GameSettingsAccepted;
    public Action<int, int> GameFieldRawSizeChanged;
    public Action<int, int> GameFieldColumnSizeChanged;

    private int _rowSize = 4;
    private int _columnSize = 6;

    [SerializeField]
    private TextMeshProUGUI _rowSizeText;
    [SerializeField]
    private TextMeshProUGUI _columnSizeText;
    [SerializeField]
    private TextMeshProUGUI _attentionText;

    [SerializeField]
    private SpriteRenderer _background;

    [SerializeField]
    private Canvas _settingsCanvas;

    [SerializeField]
    private Button _bearsSet;
    [SerializeField]
    private Button _cakesSet;
    [SerializeField]
    private Button _stuffSet;

    private Coroutine _currentSetActiveAttentionCoruotine;

    private void Awake()
    {
        _attentionText.enabled = false;
        PlayerPrefs.SetInt(PlayerSettingsConst.GAME_FIELD_ROW, 4);
        PlayerPrefs.SetInt(PlayerSettingsConst.GAME_FIELD_COLUMN, 6);
        PlayerPrefs.Save();
    }
    #region public methods, use on ScreenSettings Menu, setup in Unity Editor. Responsibility: change gameField sizes
    [UsedImplicitly]
    public void IncreaseRowSize()
    {
        if (_rowSize < 5)
        {
            _rowSize++;
        }
        else
        {
            if (_currentSetActiveAttentionCoruotine != null)
            {
                StopCoroutine(_currentSetActiveAttentionCoruotine);
                _attentionText.enabled = false;
            }
            _currentSetActiveAttentionCoruotine = StartCoroutine(ActivateAttentionText());
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
            if (_currentSetActiveAttentionCoruotine != null)
            {
                StopCoroutine(_currentSetActiveAttentionCoruotine);
                _attentionText.enabled = false;
            }
            _currentSetActiveAttentionCoruotine = StartCoroutine(ActivateAttentionText());
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
            if (_currentSetActiveAttentionCoruotine != null)
            {
                StopCoroutine(_currentSetActiveAttentionCoruotine);
                _attentionText.enabled = false;
            }
            _currentSetActiveAttentionCoruotine = StartCoroutine(ActivateAttentionText());
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
            if (_currentSetActiveAttentionCoruotine != null)
            {
                StopCoroutine(_currentSetActiveAttentionCoruotine);
                _attentionText.enabled = false;
            }
            _currentSetActiveAttentionCoruotine = StartCoroutine(ActivateAttentionText());
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
    public void SaveBearsSet()
    {
        PlayerPrefs.SetInt(PlayerSettingsConst.PLAYING_SET, 1);
        PlayerPrefs.Save();
    }
    [UsedImplicitly]
    public void SaveCakesSet()
    {
        PlayerPrefs.SetInt(PlayerSettingsConst.PLAYING_SET, 2);
        PlayerPrefs.Save();
    }
    [UsedImplicitly]
    public void SaveStuffSet()
    {
        PlayerPrefs.SetInt(PlayerSettingsConst.PLAYING_SET, 3);
        PlayerPrefs.Save();
    }
    #endregion
    private IEnumerator ActivateAttentionText()
    {
        _attentionText.enabled = true;
        yield return new WaitForSeconds(0.4f);
        _attentionText.enabled = false;
    }

}
