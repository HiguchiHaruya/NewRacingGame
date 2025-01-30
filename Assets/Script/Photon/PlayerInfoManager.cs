using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInfoManager : MonoBehaviourPunCallbacks
{
    public static PlayerInfoManager Instance;
    private Dictionary<int, string> _playerData = new Dictionary<int, string>();
    private void Awake() { Instance = this; }
    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            AssignPlayerNumber();
        }
    }
    void AssignPlayerNumber()
    {
        int playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        string playerName = PhotonNetwork.NickName;
        photonView.RPC("RegisterPlayer", RpcTarget.All, playerNumber, playerName);
    }
    [PunRPC]
    public void RegisterPlayer(int num, string name)
    {
        if (!_playerData.ContainsKey(num))
        {
            _playerData[num] = name;
        }
    }
}
