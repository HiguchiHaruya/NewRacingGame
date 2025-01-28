using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;

public class WheelController : Vehicle, ICar
{
    [SerializeField]
    private float _turnSpeed = 65f;
    [SerializeField]
    private float _driftAngle = 10f;
    [SerializeField]
    private float _tiltSpeed = 5f;
    [SerializeField]
    WheelCollider _frontRight, _frontLeft, _rearRight, _rearLeft;
    Rigidbody _rb;
    Transform _carbody;
    private int _firstRun = 0;
    private float _forwardInput;
    private float _sideInput;
    public float Speed { get; private set; }
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        _carbody = this.transform;
        _rb = GetComponent<Rigidbody>();
        RegisterTire();

        InputReader.Instance.OnMoveForwardAsObservable.Subscribe(context =>
        {
            _forwardInput = context.ReadValue<float>();
        }).AddTo(this);

        InputReader.Instance.OnMoveBackAsObservable.Subscribe(context =>
        {
            _forwardInput -= context.ReadValue<float>();
        }).AddTo(this);

        InputReader.Instance.OnMoveRightAsObservable.Subscribe(context =>
        {
            _sideInput = context.ReadValue<float>();
        }).AddTo(this);

        InputReader.Instance.OnMoveLeftAsObservable.Subscribe(context =>
        {
            _sideInput = -1 * context.ReadValue<float>();
        }).AddTo(this);
    }

    private void RegisterTire()
    {
        base.frontLeft = this._frontLeft;
        base.frontRight = this._frontRight;
        base.rearLeft = this._rearLeft;
        base.rearRight = this._rearRight;
    }

    void FixedUpdate()
    {
        Debug.Log($"‰¡input{_sideInput}");
        // if (!GameManager.Instance.IsGameStart) return;
        Drift();
        MoveSideways(_sideInput);
        Precession(_forwardInput);
        Breake();
        Acceleration(_rb);
        Speed = _rb.velocity.magnitude;
        //ApplyCarTilt(_carbody,_driftAngle,_tiltSpeed);
    }
    public override void MoveSideways(float input)
    {
        base.MoveSideways(_sideInput);
         _turnSpeed = 65f; 

        Quaternion currentRotation = _rb.rotation;
        Quaternion deltaRotation = Quaternion.Euler(0, input * _turnSpeed * Time.fixedDeltaTime, 0);
        Quaternion newRotation = currentRotation * deltaRotation;
        _rb.MoveRotation(newRotation);
    }
    public override void ApplyCarTilt(Transform carBody, float tiltAngle, float tiltSpeed)
    {
        base.ApplyCarTilt(carBody, tiltAngle, tiltSpeed);
    }
    public override void Precession(float input)
    {
        base.Precession(_forwardInput);
    }
    public override void Breake()
    {
        base.Breake();
    }
    public override void Drift()
    {
        base.Drift();
    }
    public override void Acceleration(Rigidbody rb)
    {
        base.Acceleration(rb);
    }
}