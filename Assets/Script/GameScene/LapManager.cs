using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class LapManager : MonoBehaviour
{
    ReactiveCollection<int> Triggers = new ReactiveCollection<int>();
    [SerializeField] private int totalTriggers;
    ReactiveProperty<int> _currentLap = new ReactiveProperty<int>(0);
    ReactiveProperty<bool> _isGoal = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<int> CurrentLap => _currentLap;
    public IReadOnlyReactiveProperty<bool> IsGoal => _isGoal;
    private void Start()
    {
        Triggers.ObserveCountChanged()
            .Where(count => count >= totalTriggers)
            .Subscribe(_ => LapComplate())
            .AddTo(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        var trigger = other.GetComponent<TriggerID>();
        Triggers.Add(trigger.ID);
       // Debug.Log($"^^{trigger.name}Çí âﬂ");

    }
    public void LapComplate()
    {
        if (_currentLap.Value < 3)
        {
            _currentLap.Value++;
        }
        if (_currentLap.Value >= 3)
        {
            _isGoal.Value = true;
            Debug.Log("^^Ç≤Å[ÇÈ");
        }
        ResetTriggers();
    }
    public void ResetTriggers()
    {
        Triggers.Clear();
    }
}
