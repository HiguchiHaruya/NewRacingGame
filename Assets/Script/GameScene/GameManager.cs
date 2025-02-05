using Cinemachine;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform[] _playerSpawnPoint;
    [SerializeField] Camera[] _playerCamera;


    PhotonView _view;
    List<string> names = new List<string>() { "Car", "Car2" };
    int a = 0;
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        _view = GetComponent<PhotonView>();
    }
    private void FixedUpdate()
    {
        if (PhotonNetwork.InRoom && a == 0)
        {
            a++;
            var player = PhotonNetwork.Instantiate(names[PhotonNetwork.LocalPlayer.ActorNumber - 1], _playerSpawnPoint[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.position, Quaternion.Euler(0, 90, 0));
            var controller = player.GetComponent<WheelController>();
            var virtualCamera = player.GetComponentInChildren<CinemachineVirtualCamera>();
            _playerCamera[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.parent = player.transform;
            _playerCamera[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.position = controller.GetCameraPosition().position;
            if (_view.IsMine)
            {
                virtualCamera.Priority = 999;
                _playerCamera[PhotonNetwork.LocalPlayer.ActorNumber - 1].depth = 999;
            }
        }
    }
}

