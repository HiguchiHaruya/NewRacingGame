using Cinemachine;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using Cysharp.Threading.Tasks;
using Photon.Realtime;
public class GameManager : PunSingleton<GameManager>
{
    [SerializeField] Transform[] _playerSpawnPoint;
    [SerializeField] Camera[] _playerCamera;

    List<string> names = new List<string>() { "Car", "Car2" };
    GameObject _player;
    int _initial = 0;
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    private void FixedUpdate()
    {
        GameSetUp();
    }
    private async void GameEndAsync()
    {
        await SetGoalFlag();
        PhotonNetwork.Destroy(_player);
        //  if (!PhotonNetwork.IsMasterClient) return;
        int goalPlayers = PhotonNetwork.PlayerList.Count(p => p.CustomProperties.ContainsKey("Goal") && (bool)p.CustomProperties["Goal"]);
        if (goalPlayers >= PhotonNetwork.PlayerList.Length)
        {
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
                virtualCamera.Priority = 999;
                _playerCamera[PhotonNetwork.LocalPlayer.ActorNumber - 1].depth = 999;

                _player
                    .GetComponent<LapManager>().IsGoal
                    .Where(g => g)
                    .Subscribe(_ => GameEndAsync())
                    .AddTo(this);
            }
        }
    }
    public void SetMinute(int minute)
    {
        ExitGames.Client.Photon.Hashtable prpps = new ExitGames.Client.Photon.Hashtable { { "Minute", minute } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(prpps);
    }
    public void SetSecond(int second)
    {
        ExitGames.Client.Photon.Hashtable prpps = new ExitGames.Client.Photon.Hashtable { { "Second", second } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(prpps);
    }
    public async UniTask SetGoalFlag()
    {
        ExitGames.Client.Photon.Hashtable prpps = new ExitGames.Client.Photon.Hashtable { { "Goal", true } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(prpps);
        await UniTask.WaitUntil(() => PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Goal")  //à¯êîÇ™trueÇ…Ç»ÇÈÇ‹Ç≈ë“Ç¬
        && (bool)PhotonNetwork.LocalPlayer.CustomProperties["Goal"]);
    }
    [PunRPC]
    private void TransitResultScene()
    {
        PhotonNetwork.LoadLevel("ResultScene");
    }
}

