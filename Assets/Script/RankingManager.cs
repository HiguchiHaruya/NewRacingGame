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
    private void Start()
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
                Debug.Log($"{item.Position + 1}à  {item.DisplayName}  {item.StatValue}");
            }
            foreach (var item in _rankingData)
            {
                _rankingText.AppendLine($"{a}à  {item.Value} {ConvertSecondsToTime(item.Key)} ");
                a++;
            }
            _text.text = _rankingText.ToString();
        },
        error =>
        {
            Debug.Log("ÉGÉâÅ[Ç≈ÇΩ");
        });
    }
    private string ConvertSecondsToTime(int seconds)
    {
        int m = seconds / 60;
        int s = seconds % 60;
        return $"{m:D2} : {s:D2}";
    }
}
