using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpectatorCamera : MonoBehaviour
{
    List<CinemachineVirtualCamera> _spectatorCameras = new List<CinemachineVirtualCamera>();
    [SerializeField, Header("観戦時ボタン")] Button[] _cameraSwitchButtons;

    private void Start()
    {
        if (this.TryGetComponent<CheckResult>(out var result))
        {
            result.SpectatorMode.Where(mode => mode).Subscribe(_ => ActiveButton()).AddTo(this);
        }
    }
    public void GetCamera(CinemachineVirtualCamera[] camera)
    {
        foreach (var cam in camera)
        {
            _spectatorCameras.Add(cam);
        }
        InactiveCamera();
    }
    private void InactiveCamera()
    {
        foreach (var cam in _spectatorCameras)
        {
            cam.Priority = -99;
        }
        foreach (var button in _cameraSwitchButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    private void ActiveButton()
    {
        foreach (var button in _cameraSwitchButtons)
        {
            button.gameObject.SetActive(true);
        }
        _cameraSwitchButtons[0].onClick.AddListener(() => SwitchCamera(0));
        _cameraSwitchButtons[1].onClick.AddListener(() => SwitchCamera(1));
        _cameraSwitchButtons[2].onClick.AddListener(() => SwitchCamera(2));
    }
    /// <summary>
    /// カメラの番号を指定してね.インデックスは0スタートで
    /// </summary>
    /// <param name="num"></param>
    private void SwitchCamera(int num)
    {
        for (int i = 0; i < _spectatorCameras.Count; i++)
        {
            if (i == num)
            {
                _spectatorCameras[num].Priority = 999;
            }
            else
            {
                _spectatorCameras[num].Priority = 0;
            }
        }

    }
}
