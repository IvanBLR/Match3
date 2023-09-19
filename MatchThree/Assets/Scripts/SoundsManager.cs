using JetBrains.Annotations;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _drop;
    [SerializeField] private AudioClip _buttonClick;
    [SerializeField] private AudioClip _swapBack;

    [UsedImplicitly]
    public void ButtonClick()
    {
        _audioSource.PlayOneShot(_buttonClick);
    }
    public void DropItems()
    {
        _audioSource.PlayOneShot(_drop, 0.2f);
    }

    public void SwapBack()
    {
        _audioSource.PlayOneShot(_swapBack, 0.2f);
    }
}