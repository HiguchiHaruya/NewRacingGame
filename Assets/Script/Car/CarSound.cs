using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSound : MonoBehaviour
{
  //  private CarState _idle = CarState.Idle;
    [SerializeField]
    private AudioSource _carAudio;
    [SerializeField]
    private AudioSource _driftSoundAudio;
    [SerializeField]
    private AudioClip[] _clip;
    private float _maxVolume = 1.0f;
    private float _minVolume = 0.2f;
    private float _maxPitch = 2.0f;
    private float _minPitch = 0.5f;
    private bool _highSpeed = false;
    void Update()
    {
     //   Debug.Log(Vehicle.Instance.IsDrifting);
        //SelectSound();
    }

    //private void SelectSound()
    //{
    //    switch (Vehicle.Instance.GetCurrentState())
    //    {
    //        case CarState.Idle:
    //            PlayEngineSound(0);
    //            break;
    //        case CarState.Low:
    //            PlayEngineSound(1);
    //            break;
    //        case CarState.High:
    //            PlayEngineSound(2);
    //            break;
    //    }
    //}

    private void PlayEngineSound(int index)
    {
        if (_carAudio == null) return;
        _carAudio.Stop();
        _carAudio.clip = _clip[index];
        _carAudio.Play();
    }
    private void StopEngineSound()
    {
        if(_carAudio == null) return;
        _carAudio.Stop();
    }
    private void PlayDriftSound()
    {
        if(_driftSoundAudio == null) return;
        _driftSoundAudio.Play();
    }
}
