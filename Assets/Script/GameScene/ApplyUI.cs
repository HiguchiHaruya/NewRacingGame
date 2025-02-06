using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using LitMotion;
public class ApplyUI : MonoBehaviour
{
    [SerializeField] TMP_Text _timeText;
    //[SerializeField] TMP_Text _lapText;
    private void Start()
    {
        TimeKeeper.Instance.Minutes
            .CombineLatest(TimeKeeper.Instance.Seconds, (m, s) => $"{m:D2}:{s:D2}") //“ñ‚Â‚ÌObservable‚ðŒ‹‡‚µ‚ÄV‚µ‚¢’l‚ðì‚ê‚é‚ç‚µ‚¢
            .Subscribe(time => _timeText.text = time)
            .AddTo(this);
        //for (int i = 0; i < _players.Length; i++)
        //{
        //    if (_players[i].TryGetComponent<LapManager>(out var lap))
        //    {
        //        lap
        //            ._currentLap
        //            .Subscribe(l => _lapText.text = $"{l.ToString()} / 3")
        //            .AddTo(this);
        //    }
        //}
    }
    private void Update()
    {
       // Debug.Log($"{_players[0].GetComponent<LapManager>().CurrentLap.Value}"); 
    }
}
