using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyPlayerList : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text _playerListText;
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
        UpdatePlayerList();
    }
    void UpdatePlayerList()
    {
        _playerListText.text = "プレイヤー一覧 : \n";
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (string.IsNullOrEmpty(player.NickName))
            {
                _playerListText.text += $"Player {player.ActorNumber} : 名無しさん";
                return;
            }
            _playerListText.text += $"Player {player.ActorNumber} : {player.NickName}";
        }
    }
}
