using Cinemachine;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using Cysharp.Threading.Tasks;
using Photon.Realtime;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine.UI;
using LitMotion;
using TMPro;
public class GameManager : PunSingleton<GameManager>
{
    [SerializeField] Transform[] _playerSpawnPoint;
    [SerializeField] Camera[] _playerCamera;
    [SerializeField] TMP_Text _countDownText;
    ReactiveProperty<bool> _isGameStart = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> IsGameStart => _isGameStart;

    int _minute;
    int _second;
    List<string> names = new List<string>() { "Car", "Car2" };
    GameObject _player;
    int _initial = 0;
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        CountDown();
    }
    private void OnDestroy()
    {
        PhotonNetwork.Disconnect();
    }
    private void FixedUpdate()
    {
        GameSetUp();
    }
    private async void CountDown()
    {
        await UniTask.Delay(1000);
        TextAnimation("3");
        await UniTask.Delay(1000);
        TextAnimation("2");
        await UniTask.Delay(1000);
        TextAnimation("1");
        await UniTask.Delay(1000);
        TextAnimation("スタート!!!!");
        _isGameStart.Value = true;
        await UniTask.Delay(500);
        _countDownText.gameObject.SetActive(false);
    }
    private void TextAnimation(string text)
    {
        LMotion.Create(0, text.Length, 0.5f)
            .Bind(value =>
            {
                _countDownText.text = text.Substring(0, value);
            })
            .AddTo(this);
    }
    private async void GameEndAsync()
    {
        await SetGoalFlag();
        int goalPlayers = PhotonNetwork.PlayerList.Count(p => p.CustomProperties.ContainsKey("Goal") && (bool)p.CustomProperties["Goal"]);
        if (goalPlayers >= PhotonNetwork.PlayerList.Length)
        {
            await ResultTimeToPlayFabAsync();
            //   PhotonNetwork.Destroy(_player);
            photonView.RPC("TransitResultScene", RpcTarget.All);
        }
    }
    private void GameSetUp()
    {
        if (PhotonNetwork.InRoom && _initial == 0)
        {
            _initial++;
            _player = PhotonNetwork.Instantiate(names[PhotonNetwork.LocalPlayer.ActorNumber - 1], _playerSpawnPoint[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.position, Quaternion.Euler(0, 90, 0));
            var controller = _player.GetComponent<WheelController>();
            var virtualCamera = _player.GetComponentInChildren<CinemachineVirtualCamera>();
            _playerCamera[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.parent = _player.transform;
            _playerCamera[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.position = controller.GetCameraPosition().position;
            if (_player.GetComponent<PhotonView>().IsMine)
            {
                _player
                             .GetComponent<LapManager>().IsGoal
                             .Where(g => g)
                             .Subscribe(_ => GameEndAsync())
                             .AddTo(this);
                virtualCamera.Priority = 999;
                _playerCamera[PhotonNetwork.LocalPlayer.ActorNumber - 1].depth = 999;
                _playerCamera[PhotonNetwork.LocalPlayer.ActorNumber - 1].GetComponent<AudioListener>().enabled = true;
                _player.GetComponent<AudioSource>().enabled = true;
            }
            else
            {
                _playerCamera[PhotonNetwork.LocalPlayer.ActorNumber - 1].GetComponent<AudioListener>().enabled = false;
                _player.GetComponent<AudioSource>().enabled = false;
            }
        }
    }
    private async UniTask ResultTimeToPlayFabAsync()
    {
        await UniTask.WaitUntil(() => PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Minute") && PhotonNetwork.LocalPlayer.CustomProperties["Minute"] != null);
        await UniTask.WaitUntil(() => PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Second") && PhotonNetwork.LocalPlayer.CustomProperties["Second"] != null);
        int totalSeconds = ConvertTimeToScore(_minute, _second);
        var tcs = new UniTaskCompletionSource<bool>();
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate{StatisticName = "RaceTime",Value = totalSeconds}
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request,
            result =>
            {
                tcs.TrySetResult(true);
                Debug.Log($"{totalSeconds} タイム送信完了");
            },
            error =>
            {
                tcs.TrySetException(new System.Exception($"タイム送信失敗{error.ErrorMessage}"));
                Debug.Log("タイム送信失敗");
            });
        await tcs.Task;
        Debug.Log("タイム送信完了");
    }
    private int ConvertTimeToScore(int m, int s)
    {
        return (m * 60) + s;
    }
    public void SetMinute(int minute)
    {
        _minute = minute;
        ExitGames.Client.Photon.Hashtable prpps = new ExitGames.Client.Photon.Hashtable { { "Minute", minute } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(prpps);
    }
    public void SetSecond(int second)
    {
        _second = second;
        ExitGames.Client.Photon.Hashtable prpps = new ExitGames.Client.Photon.Hashtable { { "Second", second } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(prpps);
    }
    public async UniTask SetGoalFlag()
    {
        ExitGames.Client.Photon.Hashtable prpps = new ExitGames.Client.Photon.Hashtable { { "Goal", true } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(prpps);
        await UniTask.WaitUntil(() => PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Goal")  //引数がtrueになるまで待つ
        && (bool)PhotonNetwork.LocalPlayer.CustomProperties["Goal"]);
    }
    [PunRPC]
    private void TransitResultScene()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        Debug.Log("遷移！");
        PhotonNetwork.LoadLevel("ResultScene");
    }
}

