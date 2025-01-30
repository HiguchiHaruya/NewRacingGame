using Cinemachine;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public string playerName { private get; set; }
    private void Start()
    {
        PhotonNetwork.Instantiate("Car", new Vector3(-397.200012f, 0.109999999f, -6.67999983f), Quaternion.Euler(0, 90, 0));
    }
    private void Goal()
    {
        SceneTransitionManager.Instance.LoadSceneAsync("ResultScene");
    }
    private void LeaveGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneTransitionManager.Instance.LoadSceneAsync("StartScene");
        }
    }
}

