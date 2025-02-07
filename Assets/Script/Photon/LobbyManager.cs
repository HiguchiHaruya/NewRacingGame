using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Cysharp.Threading.Tasks;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text _statusText;
    [SerializeField] TMP_Text _readyText;
    [SerializeField] TMP_Text _TestText;
    [SerializeField] int _maxPlayer = 4;
    [SerializeField] Button _readyButton;
    [SerializeField] InputField _roomNameInputField;
    [SerializeField] Button _submitButton;
    [SerializeField] GameObject _panel;
    private string _status;
    private UniTaskCompletionSource<bool> _joinRoomTask;
    private UniTaskCompletionSource<string> _inputTask;
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
        _panel.SetActive(false);
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
    public override async void OnJoinedLobby()
    {
        _status = "ロビーに接続完了。ルームに接続します";
        _panel.gameObject.SetActive(true);
        var name = await WaitInputField();
        PhotonNetwork.JoinOrCreateRoom(name, new RoomOptions { MaxPlayers = (byte)_maxPlayer, IsVisible = true }, TypedLobby.Default);
    }
    private async UniTask<string> WaitInputField()
    {
        _inputTask = new UniTaskCompletionSource<string>();
        _submitButton.onClick.AddListener(() => _inputTask.TrySetResult(_roomNameInputField.text));
        string result = await _inputTask.Task;
        _submitButton.onClick.RemoveAllListeners();
        return result;
    }
    //private async UniTask<bool> SpecifiedRoom(string roomName)
    //{
    //    _joinRoomTask = new UniTaskCompletionSource<bool>();
    //    PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = (byte)_maxPlayer, IsVisible = true }, TypedLobby.Default);
    //    return await _joinRoomTask.Task;
    //}

    public override void OnJoinedRoom()
    {
        _status = "ルームに接続完了。全員が準備完了したらゲームを開始します";
        _readyButton.interactable = true;
        _panel.gameObject.SetActive(false);
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
