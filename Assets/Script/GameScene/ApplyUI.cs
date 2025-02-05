using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using LitMotion;
public class ApplyUI : MonoBehaviour
{
    [SerializeField] TMP_Text _timeText;
    [SerializeField] TMP_Text _lapText;
    [SerializeField] TMP_Text testText;
    private void Start()
    {
        TimeKeeper.Instance.Minutes
            .CombineLatest(TimeKeeper.Instance.Seconds, (m, s) => $"{m:D2}:{s:D2}") //“ñ‚Â‚ÌObservable‚ðŒ‹‡‚µ‚ÄV‚µ‚¢’l‚ðì‚ê‚é‚ç‚µ‚¢
            .Subscribe(time => _timeText.text = time)
            .AddTo(this);

        //LapManager.Instance.CurrentLap
        //    .Subscribe(lap => _lapText.text = $"{lap.ToString()} / 3")
        //    .AddTo(this);

        
    }
}
