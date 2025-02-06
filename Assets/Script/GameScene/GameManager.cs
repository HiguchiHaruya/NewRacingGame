using Cinemachine;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
public class GameManager : Singleton<GameManager>
{
    [SerializeField] Transform[] _playerSpawnPoint;
    [SerializeField] Camera[] _playerCamera;

    List<string> names = new List<string>() { "Car", "Car2" };
    int a = 0;
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
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
            if (player.GetComponent<PhotonView>().IsMine)
            {
                virtualCamera.Priority = 999;
                _playerCamera[PhotonNetwork.LocalPlayer.ActorNumber - 1].depth = 999;
            }
        }
    }
    public void SetMinute(int minute)
    {
        ExitGames.Client.Photon.Hashtable prpps = new ExitGames.Client.Photon.Hashtable { { "Minute", minute } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(prpps);
    }
    public void SetSecond(int second)
    {
        ExitGames.Client.Photon.Hashtable prpps = new ExitGames.Client.Photon.Hashtable { { "Second", second } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(prpps);
    }
}

