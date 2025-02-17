using Cysharp.Threading.Tasks;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using PlayFab;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Text;

public class RankingManager : MonoBehaviour
{
    [SerializeField] TMP_Text _text;
    SortedDictionary<int, string> _rankingData = new SortedDictionary<int, string>();
    List<int> _order = new List<int>();
    int a = 1;
    StringBuilder _rankingText = new StringBuilder();
    private async void Start()
    {
        await EnsureLoggin(); //ログイン状態を確認。ログイン出来ていなかったらログインする
        GetRanking();
    }

    private void GetRanking()
    {
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StatisticName = "RaceTime",
            MaxResultsCount = 5,
            StartPosition = 0,
        },
        result =>
        {
            foreach (var item in result.Leaderboard)
            {
                _rankingData.Add(item.StatValue, item.DisplayName);
                _order.Add(item.Position + 1);
                Debug.Log($"{item.Position + 1}位 {item.DisplayName}  {item.StatValue}");
            }
            foreach (var item in _rankingData)
            {
                _rankingText.AppendLine($"{a}位 {item.Value} {ConvertSecondsToTime(item.Key)} ");
                a++;
            }
            TextAnimation.Instance.LTextAnimation(_rankingText.ToString(), _text);
        },
        error =>
        {
            Debug.Log("エラーでた");
        });
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
            error => Debug.Log("ログイン失敗"));
        await tcs.Task;
    }
    private string ConvertSecondsToTime(int seconds)
    {
        int m = seconds / 60;
        int s = seconds % 60;
        return $"{m:D2} : {s:D2}";
    }
}
