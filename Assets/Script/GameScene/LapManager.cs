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
    ReactiveProperty<bool> _isGoal = new ReactiveProperty<bool>(false);
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
        var trigger = other.GetComponent<TriggerID>();
        Triggers.Add(trigger.ID);
        // Debug.Log($"^^{trigger.name}‚ð’Ê‰ß");

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
    private void FixedUpdate()
    {
        if (!_view.IsMine) return;
        if (Input.GetKeyDown(KeyCode.B))
        {
            _currentLap.Value++;
            Debug.Log(CurrentLap.Value);
        }
    }
}
