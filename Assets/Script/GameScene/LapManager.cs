using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class LapManager : MonoBehaviour
{
    PhotonView _view;
    ReactiveCollection<int> Triggers = new ReactiveCollection<int>();
    [SerializeField] private int totalTriggers;
    ReactiveProperty<int> _currentLap = new ReactiveProperty<int>(3);
    public ReactiveProperty<bool> _isGoal = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<int> CurrentLap => _currentLap;
    public IReadOnlyReactiveProperty<bool> IsGoal => _isGoal;
    private void Start()
    {
        _view = GetComponent<PhotonView>();
        if (!_view.IsMine) return;
        Triggers.ObserveCountChanged()
            .Where(count => count >= totalTriggers)
            .Subscribe(_ => LapComplate())
            .AddTo(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!_view.IsMine) return;
        if (other.TryGetComponent<TriggerID>(out var triggerID) && !Triggers.Contains(triggerID.ID))
        {
            Triggers.Add(triggerID.ID);
        }
    }
    public void LapComplate()
    {
        if (!_view.IsMine) return;
        if (_currentLap.Value < 3)
        {
            _currentLap.Value++;
        }
        if (_currentLap.Value >= 3)
        {
            _isGoal.Value = true;
        }
        ResetTriggers();
    }
    public void ResetTriggers()
    {
        if (!_view.IsMine) return;
        Triggers.Clear();
    }
}
