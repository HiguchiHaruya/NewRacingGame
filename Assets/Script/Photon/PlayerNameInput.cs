using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;

public class PlayerNameInput : MonoBehaviour
{
    [SerializeField] InputField _nameInput;
    [SerializeField] Button _enterButton;
    private void Start()
    {
        _enterButton.onClick.AddListener(SetPlayerName);
    }
    private async void SetPlayerName()
    {
        await EnsureLoggin();
        string inputName = _nameInput.text.Trim();
        if (!string.IsNullOrEmpty(inputName) && PlayFabClientAPI.IsClientLoggedIn())
        {
            PhotonNetwork.NickName = inputName;
            var request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = inputName,
            };
            PlayFabClientAPI.UpdateUserTitleDisplayName(request,
                result => Debug.Log($"playfabユーザーの名前を{inputName}にしました"),
                error => Debug.Log($"ニックネーム保存しっぱい{error.ErrorMessage}")
                );
            Debug.Log($"プレイヤー名を設定 : {inputName}");
        }
    }
    private async UniTask EnsureLoggin()
    {
        if (PlayFabClientAPI.IsClientLoggedIn()) return; //既にログイン済みならreturn
        var request = new PlayFab.ClientModels.LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
        };
        var tcs = new UniTaskCompletionSource<bool>();
        PlayFabClientAPI.LoginWithCustomID(request,
            result => tcs.TrySetResult(true),
            error =>
            {
                Debug.Log("ログイン失敗");
                tcs.TrySetResult(false);
            }
            );

        var success = await tcs.Task;
        if (!success)
        {

            return;
        }
    }
}
