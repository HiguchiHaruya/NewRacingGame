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
    public string playerName { private get; set; }
    private void Start()
    {
        if(!PhotonNetwork.IsMasterClient)return;
        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            PhotonNetwork.Instantiate("Car", _playerSpawnPoint[i].transform.position, Quaternion.Euler(0, 90, 0));
        }
    }
    private void Goal()
    {
    }
    private void LeaveGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
        }
    }
}

