using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using Cysharp.Threading.Tasks;
using PlayFab.ClientModels;
using PlayFab;
using LitMotion;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
public class GameManager : PunSingleton<GameManager>
{
    [SerializeField, Header("プレイヤーの初期スポーン地点")] Transform[] _playerSpawnPoint;
    [SerializeField, Header("NPC車の初期スポーン地点")] Transform[] _aiSpawnPoint;
    [SerializeField, Header("NPC車の通過ポイント")] Transform[] _checkPoint;
    [SerializeField, Header("観戦用カメラ")] CinemachineVirtualCamera[] _virtualCamera;
    [SerializeField] Camera[] _playerCamera;
    [SerializeField] TMP_Text _countDownText;
    [SerializeField] PostProcessVolume _postProcessVolume;
    ReactiveProperty<bool> _isGameStart = new ReactiveProperty<bool>(false);
    ReactiveProperty<bool> _isGameEnd = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> IsGameStart => _isGameStart;
    public IReadOnlyReactiveProperty<bool> IsGameEnd => _isGameEnd;
    private int _spawnAICount;
    int _minute;
    int _second;
    List<string> _carNames = new List<string>() { "Car", "Car2" };
    List<string> _aiNames = new List<string>() { "AICar1", "AICar2", "AICar3", "AICar4" };
    GameObject _player;
    int _initial = 0;
    private bool _isAlone;
    private void Start()
    {
        _isAlone = PhotonNetwork.PlayerList.Length < 2;
        PhotonNetwork.AutomaticallySyncScene = true;
        _spawnAICount = _aiNames.Count - PhotonNetwork.PlayerList.Length;
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
        TextAnimation("3", _countDownText);
        await UniTask.Delay(1000);
        TextAnimation("2", _countDownText);
        await UniTask.Delay(1000);
        TextAnimation("1", _countDownText);
        await UniTask.Delay(1000);
        TextAnimation("スタート!!!!", _countDownText);
        _isGameStart.Value = true;
        await UniTask.Delay(500);
        _countDownText.gameObject.SetActive(false);
    }
    public void TextAnimation(string text, TMP_Text UIText)
    {
        LMotion.Create(0, text.Length, 0.5f)
            .Bind(value =>
            {
                UIText.text = text.Substring(0, value);
            })
            .AddTo(this);
    }
    private async void GameEndAsync()
    {
        _player.GetComponent<CheckResult>().GetCamera(_playerCamera[PhotonNetwork.LocalPlayer.ActorNumber - 1]);
        _isGameEnd.Value = true;
        _player.GetComponent<CheckResult>().ChangePostEffect(_postProcessVolume);
        await SetGoalFlag();
        int goalPlayers = PhotonNetwork.PlayerList.Count(p => p.CustomProperties.ContainsKey("Goal") && (bool)p.CustomProperties["Goal"]);
        if (goalPlayers >= PhotonNetwork.PlayerList.Length)
        {
            await ResultTimeToPlayFabAsync();
            await UniTask.Delay(5000);
            photonView.RPC("TransitResultScene", RpcTarget.All);
        }
    }
    private void GameSetUp()
    {
        if (PhotonNetwork.InRoom && _initial == 0)
        {
            _initial++;
            _player = PhotonNetwork.Instantiate(_carNames[PhotonNetwork.LocalPlayer.ActorNumber - 1], _playerSpawnPoint[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.position, Quaternion.Euler(0, 90, 0));
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
                virtualCamera.Priority = 99;
                _playerCamera[PhotonNetwork.LocalPlayer.ActorNumber - 1].depth = 99;
                _playerCamera[PhotonNetwork.LocalPlayer.ActorNumber - 1].GetComponent<AudioListener>().enabled = true;
                _player.GetComponent<AudioSource>().enabled = true;
                _player.GetComponent<SpectatorCamera>().GetCamera(_virtualCamera);

                //PhotonView[] allview = FindObjectsOfType<PhotonView>();
                //foreach (var view in allview)
                //{
                //    if (view.gameObject.CompareTag("Car") && view.TryGetComponent<WheelController>(out var c))
                //    {
                //        if (view.IsMine)
                //        {
                //            c.GetCanvas().gameObject.SetActive(true);
                //        }
                //        else if (!view.IsMine)
                //        {
                //            c.GetCanvas().gameObject.SetActive(false);
                //        }
                //    }
                //}
            }
            else
            {
                _playerCamera[PhotonNetwork.LocalPlayer.ActorNumber - 1].GetComponent<AudioListener>().enabled = false;
                _player.GetComponent<AudioSource>().enabled = false;
            }
            if (PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < _spawnAICount; i++)
                {
                    var ai = PhotonNetwork.Instantiate(_aiNames[i], _aiSpawnPoint[i].position, Quaternion.Euler(0, 90, 0));
                    ai.GetComponent<AICarController>().SetWayPoint(_checkPoint);
                }
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

