using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text _statusText;
    [SerializeField] int _maxPlayer = 4;
    private string _status;
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); //サーバーに接続
    }
    private void FixedUpdate()
    {
        _statusText.text = _status;
        if (PhotonNetwork.IsMasterClient)
        {

        }
        if (Input.GetKeyDown(KeyCode.Return)) //デバッグ
        {
            _status = "ゲームを開始します";
            PhotonNetwork.LoadLevel("GameScene");
        }
    }
    public override void OnConnectedToMaster() //サーバーに接続成功した時のコールバック()
    {
        _status = "サーバーに接続完了。ロビーに接続します";
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        _status = "ロビーに接続完了。ルームに接続します";
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = (byte)_maxPlayer }, TypedLobby.Default);
    }
    public override void OnJoinedRoom()
    {
        _status = "ルームに接続完了。最大人数に達したらゲームを開始します。。";
        CheckPlayerCount(); //最大人数に達したらゲーム開始
    }
    private void GameStart()
    {
        //ゲームを開始処理

    }
    private void CheckPlayerCount()
    {
        if (PhotonNetwork.PlayerList.Length == _maxPlayer)
        {
        }
    }
}
