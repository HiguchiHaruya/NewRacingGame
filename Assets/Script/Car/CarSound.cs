using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSound : MonoBehaviour
{
    AudioSource _audioSource;
    PhotonView _view;
    [HideInInspector] public float _pitch = 1;
    private void Start()
    {
        _view = GetComponent<PhotonView>();
        _audioSource = this.GetComponent<AudioSource>();
    }
    private void FixedUpdate()
    {
        if (!_view.IsMine) return;
        _audioSource.pitch = _pitch;
    }
    public void SoundPlay()
    {
        if (!_view.IsMine) return;
        _audioSource.Play();
        _audioSource.loop = true;
    }
    public void SoundStop() => _audioSource.Stop();
}
