using System;
using JetBrains.Annotations;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _drop;
    [SerializeField] private AudioClip _buttonClick;
    [SerializeField] private AudioClip _swapBack;
    [SerializeField] private AudioClip _bomb;

    private readonly float _soundScale = 0.2f;

    [UsedImplicitly]
    public void OnButtonClick()
    {
        _audioSource.PlayOneShot(_buttonClick);
    }

    public void OnDropItems()
    {
        _audioSource.PlayOneShot(_drop, _soundScale);
    }

    public void OnBombActivate()
    {
        _audioSource.PlayOneShot(_bomb, _soundScale);
    }

    public void OnSwapBack()
    {
        _audioSource.PlayOneShot(_swapBack, _soundScale);
    }

    private void Awake()
    {
        _audioSource.Play();
    }
}