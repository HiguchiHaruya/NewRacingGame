using Cinemachine;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] Transform[] _playerSpawnPoint;
    List<string> names = new List<string>() { "Car", "Car2" };
    int a = 0;
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        //if (PhotonNetwork.InRoom)
        //{
        //    for (int i = 0; i < PhotonNetwork.LocalPlayer.ActorNumber; i++)
        //    {
        //        PhotonNetwork.Instantiate("Car", _playerSpawnPoint[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.position, Quaternion.Euler(0, 90, 0));
        //    }

        //}
    }
    private void FixedUpdate()
    {
        if (PhotonNetwork.InRoom && a == 0)
        {
            PhotonNetwork.Instantiate(names[PhotonNetwork.LocalPlayer.ActorNumber - 1], _playerSpawnPoint[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.position, Quaternion.Euler(0, 90, 0));
       //     PhotonNetwork.Instantiate(names[PhotonNetwork.LocalPlayer.ActorNumber], _playerSpawnPoint[PhotonNetwork.LocalPlayer.ActorNumber].transform.position, Quaternion.Euler(0, 90, 0));
            a++;
        }
    }
    //private void Goal()
    //{
    //}
    //private void LeaveGame()
    //{
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {
    //    }
    //}
}

