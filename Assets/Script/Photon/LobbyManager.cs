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
using PlayFab;
using PlayFab.ClientModels;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text _ramdomFailText;
    [SerializeField] TMP_Text _statusText;
    [SerializeField] TMP_Text _readyText;
    [SerializeField] TMP_Text _TestText;
    [SerializeField] int _maxPlayer = 4;
    [SerializeField] Button _readyButton;
    [SerializeField] InputField _roomNameInputField;
    [SerializeField] Button _submitButton;
    [SerializeField] Button _randomButton;
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

        //  PhotonNetwork.ConnectUsingSettings(); //サーバーに接続
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
        PhotonNetwork.AutomaticallySyncScene = true;

        _readyButton.interactable = false;
        _panel.SetActive(false);
        _readyText.text = "準備中";
        _readyButton.onClick.AddListener(SetReady);
        PhotonNetwork.JoinLobby();
    }
    public override async void OnJoinedLobby()
    {
        _status = "ロビーに接続完了。ルームに接続します";
        _panel.gameObject.SetActive(true);
        _ramdomFailText.gameObject.SetActive(false);
        _randomButton.onClick.AddListener(JoinRandomRoom);
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
    private void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public async override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("ランダムな部屋が見つかりませんでした");
        _ramdomFailText.gameObject.SetActive(true);
        await UniTask.Delay(1000);
        _ramdomFailText.gameObject.SetActive(false);
    }
    public async override void OnJoinedRoom()
    {
        _status = "ルームに接続完了。全員が準備完了したらゲームを開始します";
        await LoadNickNameFromPlayFab();
        _readyButton.interactable = true;
        _panel.gameObject.SetActive(false);
    }
    public void SetReady()
    {
        _readyButton.interactable = false;
        _readyText.text = "準備完了";
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { "IsReady", true } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
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
    private async UniTask LoadNickNameFromPlayFab()
    {
        if (!this.GetComponent<PhotonView>().IsMine) return;
        var tcs = new UniTaskCompletionSource<bool>();
        PlayFabClientAPI.GetAccountInfo(new PlayFab.ClientModels.GetAccountInfoRequest(),
            result =>
            {
                string savedNickName = result.AccountInfo.TitleInfo.DisplayName;
                if (string.IsNullOrEmpty(savedNickName))
                {
                    SaveNickName("名無しさん");
                }
                else
                {
                    SaveNickName(result.AccountInfo.TitleInfo.DisplayName);
                }
                tcs.TrySetResult(true);
            },
            error =>
            {
                Debug.Log("デフォルト名設定失敗");
                tcs.TrySetResult(false);
            });
        await tcs.Task;
    }
    private void SaveNickName(string name)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
            result => Debug.Log("デフォルト名設定完了"),
            error => Debug.Log("デフォルト名設定出来ませんでした"));
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
