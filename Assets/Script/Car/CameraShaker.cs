using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
public class CameraShaker : MonoBehaviour 
{
    PhotonView _view;
    private void Start()
    {
        _view = GetComponent<PhotonView>();
    }
    public void CameraShake()
    {
        if (_view.IsMine)
        {
            this.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
        }
    }
}
