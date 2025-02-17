using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetAudioVolume : MonoBehaviour
{
    [SerializeField] Slider _volumeSlider;
    [SerializeField] Button _submitButton;
    private void Start()
    {
        _submitButton.onClick.AddListener(SubmitVolume);
        _volumeSlider.value = 0.5f;
    }
    private void SubmitVolume()
    {
        AudioManager.instance.SetVolume(_volumeSlider.value);
    }
}
