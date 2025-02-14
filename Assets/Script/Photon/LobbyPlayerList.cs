using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Cysharp.Threading.Tasks;
using PlayFab;
using Unity.VisualScripting;

public class LobbyPlayerList : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text _playerListText;
    private string _playerName;
    private async void Start()
    {
        await UniTask.WaitUntil(() => PlayFabClientAPI.IsClientLoggedIn());
        PlayFabClientAPI.GetAccountInfo(new PlayFab.ClientModels.GetAccountInfoRequest(),
       result =>
       {
           _playerName = result.AccountInfo.TitleInfo.DisplayName;
           PhotonNetwork.LocalPlayer.NickName = result.AccountInfo.TitleInfo.DisplayName;
       },
       error => Debug.Log(error.ErrorMessage));
    }
    public override void OnJoinedRoom()
    {
        UpdatePlayerList();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) //プレイヤーのプロパティが変更されたときに呼ばれるコールバック関数
    {
      //  PlayFabClientAPI.GetAccountInfo(new PlayFab.ClientModels.GetAccountInfoRequest(),
      //async result =>
      //{
      //    await UniTask.Delay(1500);
      //    _playerName = result.AccountInfo.TitleInfo.DisplayName;
      //    if (!string.IsNullOrEmpty(_playerName))
      //    {
      //        targetPlayer.NickName = _playerName;
      //    }
      //    UpdatePlayerList();
      //},
      //error => Debug.Log(error.ErrorMessage));
    }
    //async void UpdateNickNameList()
    //{
    //    await UniTask.Delay(1500);
    //    foreach (var player in PhotonNetwork.PlayerList)
    //    {
    //        if (string.IsNullOrEmpty(_playerName))
    //        {
    //            _playerListText.text += $"Player {player.ActorNumber} : 名無しさん\n";
    //            return;
    //        }
    //        _playerListText.text += $"Player {player.ActorNumber} : {player.NickName}\n";
    //    }
    //}

    async void UpdatePlayerList()
    {
        await UniTask.Delay(2000);
        _playerListText.text = "プレイヤー一覧 : \n";
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (string.IsNullOrEmpty(_playerName))
            {
                _playerListText.text += $"Player {player.ActorNumber} : 名無しさん\n";
                return;
            }
            _playerListText.text += $"Player {player.ActorNumber} : {player.NickName}\n";
        }
    }

}
