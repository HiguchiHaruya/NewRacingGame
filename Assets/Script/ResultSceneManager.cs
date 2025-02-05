using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultSceneManager : MonoBehaviour
{
    [SerializeField]
    Button _playGameAgainButton;
    [SerializeField]
    Button _transitionStartButton;
    [SerializeField]
    InputField _playerName;
    [SerializeField]
    Button _enter;
    [SerializeField]
    Text _rankingText1;
    [SerializeField]
    Text _rankingText2;
    [SerializeField]
    Text _rankingText3;
    private void Start()
    {
        _playGameAgainButton.onClick.AddListener(() => SceneTransition("GameScene"));
        _transitionStartButton.onClick.AddListener(() => SceneTransition("StartScene"));
        _enter.onClick.AddListener(AddRanking);
    }
    private void SceneTransition(string scenename)
    {
      //  Destroy(Vehicle.Instance.gameObject);
      //  Destroy(GameManager.Instance.gameObject);
     //   SceneTransitionManager.Instance.LoadSceneAsync(scenename);
    }
    private void AddRanking()
    {
        //RankingManager.Instance.AddScore(_playerName.text, GameManager.Instance.CurrentGameTime);
        if (RankingManager.Instance.RankingList.Count >= 1)
        {
            _rankingText1.text = $"1位 : {RankingManager.Instance.GetRankingData(0).playerName} タイム : {RankingManager.Instance.GetRankingData(0).score.ToString("F2")}";
        }
        else
        {
            _rankingText1.text = "1位 : --- タイム : ---";
        }
        if (RankingManager.Instance.RankingList.Count >= 2)
        {
            _rankingText2.text = $"2位 : {RankingManager.Instance.GetRankingData(1).playerName} タイム : {RankingManager.Instance.GetRankingData(1).score.ToString("F2")}";
        }
        else
        {
            _rankingText2.text = "2位 : --- タイム : ---";
        }
        if (RankingManager.Instance.RankingList.Count >= 3)
        {
            _rankingText3.text = $"3位 : {RankingManager.Instance.GetRankingData(2).playerName} タイム : {RankingManager.Instance.GetRankingData(2).score.ToString("F2")}";
        }
        else
        {
            _rankingText3.text = "3位 : --- タイム : ---";
        }
    }
}
