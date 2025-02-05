using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text _statusText;
    [SerializeField] TMP_Text _readyText;
    [SerializeField] TMP_Text _TestText;
    [SerializeField] int _maxPlayer = 4;
    [SerializeField] Button _readyButton;
    private string _status;
    List<int> _readyPlayers = new List<int>();
    public static LobbyManager Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != null) Destroy(gameObject);

        PhotonNetwork.ConnectUsingSettings(); //サーバーに接続
    }
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        _readyButton.interactable = false;
        _readyText.text = "準備中";
        _readyButton.onClick.AddListener(SetReady);
    }
    private void FixedUpdate()
    {
        _statusText.text = _status;
        if (PhotonNetwork.CurrentRoom != null)
        {
            _TestText.text = $"Room名 : {PhotonNetwork.CurrentRoom.Name}  {PhotonNetwork.CurrentRoom.PlayerCount}人";
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
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = (byte)_maxPlayer, IsVisible = true }, TypedLobby.Default);
    }
    public override void OnJoinedRoom()
    {
        _status = "ルームに接続完了。最大人数に達したらゲームを開始します。。";
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
        //photonView.RPC("CheckAllReady", RpcTarget.All);
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { "IsReady", true } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        //CheckAllReady();
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (changedProps.ContainsKey("IsReady") == true)
        {
            _readyPlayers.Add(PhotonNetwork.LocalPlayer.ActorNumber);
            photonView.RPC("CheckAllReady", RpcTarget.All);
        }
    }
    [PunRPC]
    void CheckAllReady()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (_readyPlayers.Count == PhotonNetwork.PlayerList.Length)
        {
            Debug.Log("全員準備完了！マスタークライアントがシーンを変更します");
            PhotonNetwork.LoadLevel("GameScene");
        }
    }
}
