using System.Collections;
using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;
using System;
using Photon.Pun;

public class InputReader : MonoBehaviour, PlayerInputControls.IPlayerActionMapActions
{
    private PlayerInputControls _controls;
    private PhotonView _view;
    private Subject<InputAction.CallbackContext> _onBrakeSubject = new();
    private Subject<InputAction.CallbackContext> _onDriftSubject = new();
    private Subject<InputAction.CallbackContext> _onMoveBackSubject = new();
    private Subject<InputAction.CallbackContext> _onMoveForwardSubject = new();
    private Subject<InputAction.CallbackContext> _onMoveLeftSubject = new();
    private Subject<InputAction.CallbackContext> _onMoveRightSubject = new();
    private Subject<InputAction.CallbackContext> _onOtherSubject = new();
    private Subject<InputAction.CallbackContext> _onCameraSwitchSubject = new();

    public IObservable<InputAction.CallbackContext> OnBrakeAsObservable => _onBrakeSubject;
    public IObservable<InputAction.CallbackContext> OnDriftAsObservable => _onDriftSubject;
    public IObservable<InputAction.CallbackContext> OnMoveBackAsObservable => _onMoveBackSubject;
    public IObservable<InputAction.CallbackContext> OnMoveForwardAsObservable => _onMoveForwardSubject;
    public IObservable<InputAction.CallbackContext> OnMoveLeftAsObservable => _onMoveLeftSubject;
    public IObservable<InputAction.CallbackContext> OnMoveRightAsObservable => _onMoveRightSubject;
    public IObservable<InputAction.CallbackContext> OnOtherAsObservable => _onOtherSubject;
    public IObservable<InputAction.CallbackContext> OnCameraSwitchAsObservable => _onCameraSwitchSubject;

    private void Awake()
    {
        _view = GetComponent<PhotonView>();
        if (_view.IsMine)
        {
            _controls = InputManager.Instance._inputActions;
            _controls.PlayerActionMap.SetCallbacks(this);
            _controls.PlayerActionMap.Enable();
            Debug.Log("inputControlのセットアップが完了しました");
        }
    }

    private void OnDestroy()
    {
        if (_view.IsMine)
        {
            _controls.PlayerActionMap.Disable();
        }
    }

    public void OnBrake(InputAction.CallbackContext context) => _onBrakeSubject.OnNext(context);
    public void OnDrift(InputAction.CallbackContext context) => _onDriftSubject.OnNext(context);
    public void OnMoveBack(InputAction.CallbackContext context) => _onMoveBackSubject.OnNext(context);
    public void OnMoveForward(InputAction.CallbackContext context) => _onMoveForwardSubject.OnNext(context);
    public void OnMoveLeft(InputAction.CallbackContext context) => _onMoveLeftSubject.OnNext(context);
    public void OnMoveRight(InputAction.CallbackContext context) => _onMoveRightSubject.OnNext(context);
    public void OnOther(InputAction.CallbackContext context) => _onOtherSubject.OnNext(context);
    public void OnSwitchCamera(InputAction.CallbackContext context) => _onCameraSwitchSubject.OnNext(context);
}
