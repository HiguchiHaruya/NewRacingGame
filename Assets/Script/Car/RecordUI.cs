using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UniRx;

public class RecordUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _lapText;
    PhotonView _photonView;
    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        if (_photonView.IsMine)
        {
            _lapText.enabled = true;
            _lapText.gameObject.SetActive(true);
            if (this.TryGetComponent<LapManager>(out var lap))
            {
                lap
                    .CurrentLap
                    .Subscribe(l => _lapText.text = $"{l.ToString()} / 3")
                    .AddTo(this);
            }
        }
        else
        {
            _lapText.enabled = false;
            _lapText.gameObject.SetActive(false);
        }
    }
}
