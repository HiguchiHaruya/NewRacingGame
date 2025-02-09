using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TimeKeeper : Singleton<TimeKeeper>
{
    [SerializeField] GameObject[] _players;
    private int _minutes = 0;
    private int _seconds = 0;
    public IReadOnlyReactiveProperty<int> Minutes => _minutesReactive;
    public IReadOnlyReactiveProperty<int> Seconds => _secondsReactive;
    private ReactiveProperty<int> _minutesReactive = new ReactiveProperty<int>(0);
    private ReactiveProperty<int> _secondsReactive = new ReactiveProperty<int>(0);
    private void Start()
    {
        Observable.Interval(System.TimeSpan.FromSeconds(1f))
            .Subscribe(_ => IncrementTime()) //1ïbÇ≤Ç∆Ç…éûä‘Çâ¡éZÇµÇƒÇ¢Ç≠
            .AddTo(this);

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (_players[PhotonNetwork.LocalPlayer.ActorNumber - 1].TryGetComponent<LapManager>(out var lapManager))
            {
                Observable.EveryUpdate().Subscribe(_ => Debug.Log(lapManager._isGoal.Value)).AddTo(this);

                lapManager._isGoal
                    .Where(isGoal => isGoal)
                    .Subscribe(_ =>
                    {
                        Debug.Log("åƒÇ—èoÇ∑");
                        GameManager.Instance.SetMinute(Minutes.Value);
                        GameManager.Instance.SetSecond(Seconds.Value);
                    })
                    .AddTo(this);
            }
        }
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
