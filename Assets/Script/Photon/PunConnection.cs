using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PunConnection : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); //サーバーに接続
    }
    public override void OnConnectedToMaster() //サーバーに接続成功した時に呼ばれる
    {
        PhotonNetwork.JoinOrCreateRoom("Room1", new RoomOptions(), TypedLobby.Default);//Room1というルームを作成。既にあれば参加
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
    }
}
