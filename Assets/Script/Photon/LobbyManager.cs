using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text _statusText;
    [SerializeField] TMP_Text _readyText;
    [SerializeField] int _maxPlayer = 4;
    [SerializeField] Button _readyButton;
    private string _status;
    List<int> _readyPlayers = new List<int>();
    private void Start()
    {
        _readyButton.interactable = false;
        PhotonNetwork.ConnectUsingSettings(); //サーバーに接続
        _readyText.text = "準備中";
        _readyButton.onClick.AddListener(SetReady);
    }
    private void FixedUpdate()
    {
        _statusText.text = _status;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameStart();
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
        _readyPlayers.Add(PhotonNetwork.LocalPlayer.ActorNumber);
        _readyButton.interactable = true;
    }
    private void GameStart()
    {
        //ゲームを開始処理
        Debug.Log("移行します");
        StartCoroutine(SceneTransitionManager.Instance.LoadSceneAll("GameScene"));
    }
    public void SetReady()
    {
        _readyButton.interactable = false;
        _readyText.text = "準備完了";
        photonView.RPC("CheckAllReady", RpcTarget.All);
    }
    [PunRPC]
    void CheckAllReady()
    {
        if (_readyPlayers.Count == PhotonNetwork.PlayerList.Length)
        {
            GameStart();
        }
    }
}
