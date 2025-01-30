using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; //ファイル操作の為に必要
using Newtonsoft.Json; //jsonを使う為に必要

public class RankingManager : Singleton<RankingManager>
{
    [SerializeField]
    private bool _clearRankingData = false;
    [System.Serializable] 
    public class PlayerScore
    {
        public string playerName;
        public float score;
    }
    private List<PlayerScore> rankingList = new List<PlayerScore>();
    public List<PlayerScore> RankingList => rankingList;
    private string filePath; //保存先
    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "ranking.json"); //jsonの保存先ファイルパスをしていする
        if (_clearRankingData) ClearRankingData();
        LoadRanking(); //起動時に保存されているrankingをロード
    }
    public void AddScore(string playerName, float score)
    {
        PlayerScore newScore = new PlayerScore { playerName = playerName, score = score };
        rankingList.Add(newScore); //追加してく
        rankingList.Sort((a, b) => a.score.CompareTo(b.score)); //昇順ソート
        if (rankingList.Count > 3)
        {
            rankingList.RemoveAt(3);
        }
        SaveRanking();
    }
    /// <summary>ランキングデータ消去メソッド</summary>
    private void ClearRankingData()
    {
       // Debug.Log("ランキングデータ消去!!!!!");
        rankingList.Clear();
        List<PlayerScore> empty = new List<PlayerScore>();
        string json = JsonConvert.SerializeObject(empty, Formatting.Indented); //Formatting.Indented → jsonファイルに改行とか入れて読みやすくしてくれる
        File.WriteAllText(filePath, json);
        SaveRanking();
    }
    ///<summary> ランキングデータセーブ</summary>
    public void SaveRanking()
    {
        string jsonData = JsonConvert.SerializeObject(rankingList, Formatting.Indented); //リストをjson形式に変換する
        File.WriteAllText(filePath, jsonData); //ファイルにjsonデータを書き込む
      //  Debug.Log("rankingが保存されました！");
    }
    /// <summary> ランキングデータ読み込む</summary>
    public void LoadRanking()
    {
        if (!File.Exists(filePath)) return; //ファイルが存在しない場合はreturn
        string jsonData = File.ReadAllText(filePath); //読み込む
        rankingList = JsonConvert.DeserializeObject<List<PlayerScore>>(jsonData); //Jsonデータをリストに変換してrankingListに入れる
       // Debug.Log("rankingがロードされました！");
    }
    /// <summary>ランキングデータを渡す</summary>
    /// <param name="rank"></param>
    /// <returns>ランキングデータ</returns>
    public PlayerScore GetRankingData(int rank)
    {
        return rankingList[rank];
    }
}
