using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _drop;
    [SerializeField] private AudioClip _buttonClick;

    [UsedImplicitly]
    public void ButtonClick()
    {
        _audioSource.PlayOneShot(_buttonClick);
    }

    public void DropItems()
    {
        _audioSource.PlayOneShot(_drop, 0.4f);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            DropItems();
    }
}