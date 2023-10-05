using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGame : MonoBehaviour
{
    [SerializeField] private Yandex _yandexSDK;
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _image;

    private readonly float _loadDuration = 3.5f;
    private float _loadingTime;

    private void Awake()
    {
        _slider.gameObject.SetActive(true);
    }

    private void Update()
    {
        _loadingTime += Time.deltaTime;
        var progress = _loadingTime / _loadDuration;
        ChangeLoadsValue(progress);
        if (_loadingTime >= _loadDuration)
        {
            DontDestroyOnLoad(_yandexSDK);
            SceneManager.LoadScene(1);
        }
    }

    private void ChangeLoadsValue(float value)
    {
        _slider.value = value;
        var color = _image.color;
        color.a = value;
        _image.color = color;
    }
}