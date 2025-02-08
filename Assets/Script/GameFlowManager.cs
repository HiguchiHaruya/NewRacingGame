using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Cysharp.Threading.Tasks;
using UniRx;
using Photon.Pun;
using System;

public class GameFlowManager : MonoBehaviour
{
    private async void Start()
    {
        try
        {
            await PlayFabLogin();
            await ConnectToPhoton();
        }
        catch(Exception ex)
        {
            Debug.LogError($"途中でエラーでた! {ex.Message}");
        }
    }
    private async UniTask PlayFabLogin() //PlayFabログイン
    {
        var tcs = new UniTaskCompletionSource<bool>();
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request,
            result => tcs.TrySetResult(true),
            error => tcs.TrySetException(new System.Exception("ログイン失敗"))
            );
        await tcs.Task;
        Debug.Log("PlayFabログイン成功");
    }
    private async UniTask ConnectToPhoton() //Photonログイン
    {
        var tcs = new UniTaskCompletionSource<bool>();
        PhotonNetwork.AuthValues = new Photon.Realtime.AuthenticationValues(SystemInfo.deviceUniqueIdentifier);
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "jp";
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.NetworkingClient.EventReceived += eventData =>
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                tcs.TrySetResult(true);
            }
        };
        await tcs.Task;
        Debug.Log("Photonログイン成功");
    }
}
