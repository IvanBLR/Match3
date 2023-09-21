using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_controller : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] _levels;
   // [SerializeField] private Button _restartButton;
    private int _previousIndex = 0;

    public void ActivateChoosenLevel(int index)
    {
        _levels[_previousIndex].gameObject.SetActive(false);
        _levels[index].gameObject.SetActive(true);
        _previousIndex = index;
    }

    public void DeactivateChoosenViewUILevels()
    {
        for (int i = 0; i < _levels.Length; i++)
        {
            _levels[i].gameObject.SetActive(false);
        }
    }

    [UsedImplicitly]
    public void BackToMainMenu()
    {
        gameObject.SetActive(false);
        SceneManager.LoadScene(0);
    }

    private void Awake()
    {
        for (int i = 0; i < _levels.Length; i++)
        {
            _levels[i].gameObject.SetActive(false);
        }
        int index = PlayerPrefs.GetInt(SettingsConstant.PLAYING_SET, 0);
        _levels[index].gameObject.SetActive(true);
    }
}