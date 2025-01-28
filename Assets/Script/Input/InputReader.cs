using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;
public class InputReader : Singleton<InputReader>, PlayerInputControls.IPlayerActionMapActions
{
 
    private PlayerInputControls _contrls;
    private float _forwardInput;
    private float _backwardInput;
    private float _leftInput;
    private float _rightInput;
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
    void Start()
    {
        _contrls = new PlayerInputControls();
        _contrls.PlayerActionMap.SetCallbacks(this);
        _contrls.PlayerActionMap.Enable();
    }
    public void OnBrake(InputAction.CallbackContext context)
    {
        _onBrakeSubject.OnNext(context);
    }

    public void OnDrift(InputAction.CallbackContext context)
    {
        _onDriftSubject.OnNext(context);
    }

    public void OnMoveBack(InputAction.CallbackContext context)
    {
        _onMoveBackSubject.OnNext(context);
    }

    public void OnMoveForward(InputAction.CallbackContext context)
    {
        _onMoveForwardSubject.OnNext(context);
    }

    public void OnMoveLeft(InputAction.CallbackContext context)
    {
        _onMoveLeftSubject.OnNext(context);
    }

    public void OnMoveRight(InputAction.CallbackContext context)
    {
        _onMoveRightSubject.OnNext(context);
    }

    public void OnOther(InputAction.CallbackContext context)
    {
        _onOtherSubject.OnNext(context);
    }

    public void OnSwitchCamera(InputAction.CallbackContext context)
    {
        _onCameraSwitchSubject.OnNext(context);
    }

}
