using System;
using System.Collections;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_controller : MonoBehaviour
{
    public Action<int, int> GameFieldRawSizeChanged;
    public Action<int, int> GameFieldColumnSizeChanged;

  //  [SerializeField] private SpriteRenderer[] _levels;
  //  [SerializeField] private Sprite[] _backgroundSprites;
    [SerializeField] private SpriteRenderer _background;
    [SerializeField] private SpriteRenderer _soundOn;
    [SerializeField] private SpriteRenderer _soundOff;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private TextMeshProUGUI _rowSizeText;
    [SerializeField] private TextMeshProUGUI _columnSizeText;
    [SerializeField] private TextMeshProUGUI _attentionText;

    [SerializeField] private Canvas _settingsCanvas;

    private Coroutine _currentSetActiveAttention;
    private int _previousIndex;
    private int _rowSize = 8;
    private int _columnSize = 7;
    private bool _isSoundOn = true;
    
    #region public methods setup in Unity Editor. Responsibility: change gameField sizes

    [UsedImplicitly]
    public void IncreaseRowSize()
    {
        if (_rowSize < 12)// it was 5
            _rowSize++;
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
        PrefsManager.SaveDataInt(PlayingSettingsConstant.GAME_FIELD_ROW, _rowSize);
        GameFieldRawSizeChanged?.Invoke(_rowSize, _columnSize);
    }

    [UsedImplicitly]
    public void DecreaseRowSize()
    {
        if (_rowSize > 6)
            _rowSize--;
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
        PrefsManager.SaveDataInt(PlayingSettingsConstant.GAME_FIELD_ROW, _rowSize);
        GameFieldRawSizeChanged?.Invoke(_rowSize, _columnSize);
    }

    [UsedImplicitly]
    public void IncreaseColumnSize()
    {
        if (_columnSize < 7)// it was 8
            _columnSize++;
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
        PrefsManager.SaveDataInt(PlayingSettingsConstant.GAME_FIELD_COLUMN, _columnSize);
        GameFieldColumnSizeChanged?.Invoke(_rowSize, _columnSize);
    }

    [UsedImplicitly]
    public void DecreaseColumnSize()
    {
        if (_columnSize > 6)
            _columnSize--;
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
        PrefsManager.SaveDataInt(PlayingSettingsConstant.GAME_FIELD_COLUMN, _columnSize);
        GameFieldColumnSizeChanged?.Invoke(_rowSize, _columnSize);
    }

    #endregion

    [UsedImplicitly]
    public void BackToMainMenu()
    {
        gameObject.SetActive(false);
        SceneManager.LoadScene(0);
    }
    
    [UsedImplicitly]
    public void SoundTumbler()
    {
        _audioSource.mute = _isSoundOn;
        _soundOff.gameObject.SetActive(_isSoundOn);
        _soundOn.gameObject.SetActive(!_isSoundOn);
        _isSoundOn = !_isSoundOn;
    }
  //  public void ActivateAcceptLevelView(int index)
  //  {
  //      _levels[_previousIndex].gameObject.SetActive(false);
  //      _levels[index].gameObject.SetActive(true);
//
  //      _background.sprite = _backgroundSprites[index];
//
  //      _previousIndex = index;
  //  }

  //  public void DeactivateAcceptLevelView()
  //  {
  //      for (int i = 0; i < _levels.Length; i++)
  //      {
  //          _levels[i].gameObject.SetActive(false);
  //      }
  //  }

    public void LowDownBackgroundAlpha()
    {
        var color = _background.color;
        color.a = 0.5f;
        _background.color = color;
    }

    public void StartedGame() => _settingsCanvas.enabled = false;

    private void Awake()
    {
      //  for (int i = 0; i < _levels.Length; i++)
      //  {
      //      _levels[i].gameObject.SetActive(false);
      //  }
//
      //  _previousIndex = PlayerPrefs.GetInt(PlayingSettingsConstant.PLAYING_SET, 0);
      //  _levels[_previousIndex].gameObject.SetActive(true);
      //  _background.sprite = _backgroundSprites[_previousIndex];

        _attentionText.enabled = false;
        PrefsManager.SaveDataInt(PlayingSettingsConstant.GAME_FIELD_ROW, _rowSize);
        PrefsManager.SaveDataInt(PlayingSettingsConstant.GAME_FIELD_COLUMN, _columnSize);
    }

    private IEnumerator ActivateAttentionText()
    {
        //_attentionText.gameObject.SetActive(true);
        _attentionText.enabled = true;
        yield return new WaitForSeconds(0.5f);
        _attentionText.enabled = false;
       // _attentionText.gameObject.SetActive(false);
    }
}