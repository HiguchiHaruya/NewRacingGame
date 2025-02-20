using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.CloudScriptModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackSender : MonoBehaviour
{
    [SerializeField] Button _submitButton;
    [SerializeField] TMP_InputField _inputField;
    private async void Start()
    {
        await EnsureLoggin();
        _submitButton.onClick.AddListener(() => SendFeedBack(_inputField.text));
    }
    public void SendFeedBack(string feedback)
    {
        var request = new ExecuteFunctionRequest
        {
            FunctionName = "SendFeedback",
            FunctionParameter = new Dictionary<string, object> {
                {"feedback", feedback  },
                {"playerName",PlayFabSettings.staticPlayer.PlayFabId }
            }
        };
        Debug.Log(JsonUtility.ToJson(request));
        PlayFabCloudScriptAPI.ExecuteFunction(request,
            result => Debug.Log("アンケート送信成功"),
            error => Debug.Log($"アンケート送信失敗 {error.ErrorMessage}"));
    }
    private async UniTask<bool> EnsureLoggin()
    {
        if (PlayFabClientAPI.IsClientLoggedIn()) return true; //既にログイン済みならreturn
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
        if (success == true) { return true; }
        return false;
    }
    private string ConvertSecondsToTime(int seconds)
    {
        int m = seconds / 60;
        int s = seconds % 60;
        return $"{m:D2} : {s:D2}";
    }
}
