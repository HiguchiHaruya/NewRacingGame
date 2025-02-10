using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TimeKeeper : Singleton<TimeKeeper>
{
    //[SerializeField] GameObject[] _players;
    [SerializeField] string[] names;
    GameObject _players;
    LapManager _lpm;
    private int _minutes = 0;
    private int _seconds = 0;
    public IReadOnlyReactiveProperty<int> Minutes => _minutesReactive;
    public IReadOnlyReactiveProperty<int> Seconds => _secondsReactive;
    private ReactiveProperty<int> _minutesReactive = new ReactiveProperty<int>(0);
    private ReactiveProperty<int> _secondsReactive = new ReactiveProperty<int>(0);
    private async void Start()
    {
        Observable.Interval(System.TimeSpan.FromSeconds(1f))
            .Subscribe(_ => IncrementTime()) //1ïbÇ≤Ç∆Ç…éûä‘Çâ¡éZÇµÇƒÇ¢Ç≠
            .AddTo(this);
        //for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        //{
        await UniTask.Delay(3000);
        _players = GameObject.Find(names[PhotonNetwork.LocalPlayer.ActorNumber - 1]);
        if (_players.GetComponent<PhotonView>().IsMine)
        {
            var lm = _players.GetComponent<LapManager>();
            lm._isGoal
                .DistinctUntilChanged()
                .Where(isGoal => isGoal)
                .Subscribe(_ =>
                {
                    Debug.Log("åƒÇ—èoÇ∑");
                    GameManager.Instance.SetMinute(Minutes.Value);
                    GameManager.Instance.SetSecond(Seconds.Value);
                })
                .AddTo(this);
        }
        else
        {
            Debug.Log("LapManagerÇ†ÇËÇ‹ÇπÇÒ");
        }
        //}
    }
    private void FixedUpdate()
    {
        //Debug.Log(_lpm._isGoal);
    }
    private void IncrementTime()
    {
        _seconds += 1;
        _secondsReactive.Value = _seconds;
        if (_seconds >= 60)
        {
            _seconds = 0;
            _secondsReactive.Value = _seconds;

            _minutes += 1;
            _minutesReactive.Value = _minutes;
        }
        // Debug.Log($"åªç›ÇÃÉ^ÉCÉÄ{_minutes}m {_seconds}s");
    }
}
