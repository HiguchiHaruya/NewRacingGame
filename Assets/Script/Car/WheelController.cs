using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class WheelController : Vehicle, ICar
{
    [SerializeField]
    private float _turnSpeed = 65f;
    [SerializeField]
    private float _driftAngle = 10f;
    [SerializeField]
    private float _tiltSpeed = 5f;
    [SerializeField]
    private WheelCollider _frontRight, _frontLeft, _rearRight, _rearLeft;
    [SerializeField] CarSound _sound;
    [SerializeField]
    private Transform _cameraPosition;
    private Rigidbody _rb;
    private Transform _carbody;
    private float _forwardInput;
    private float _sideInput;
    private InputReader _inputReader;
    private PhotonView _photonView;
    private float _soundPitch = 1;
    private int _initial = 0;
    public float Speed { get; private set; }

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        if (!_photonView.IsMine)
        {
            // GetComponent<PlayerInput>().enabled = false;
            return;
        }
        _carbody = transform;
        _rb = GetComponent<Rigidbody>();
        RegisterTire();

        _inputReader = GetComponent<InputReader>();

        _inputReader.OnMoveForwardAsObservable.Subscribe(context =>
        {
            _forwardInput = context.ReadValue<float>();
        }).AddTo(this);

        _inputReader.OnMoveBackAsObservable.Subscribe(context =>
        {
            _forwardInput = -1 * context.ReadValue<float>();
        }).AddTo(this);

        _inputReader.OnMoveRightAsObservable.Subscribe(context =>
        {
            _sideInput = context.ReadValue<float>();
        }).AddTo(this);

        _inputReader.OnMoveLeftAsObservable.Subscribe(context =>
        {
            _sideInput = -1 * context.ReadValue<float>();
        }).AddTo(this);
    }
    private void OnDestroy()
    {
        PhotonNetwork.Disconnect();
    }
    private void RegisterTire()
    {
        base.frontLeft = _frontLeft;
        base.frontRight = _frontRight;
        base.rearLeft = _rearLeft;
        base.rearRight = _rearRight;
    }

    void FixedUpdate()
    {
        if (!_photonView.IsMine) return;

        Drift();
        MoveSideways(_sideInput);
        Precession(_forwardInput);
        Breake();
        Acceleration(_rb);
        Speed = _rb.velocity.magnitude;

        SetEngineSound();
    }

    private void SetEngineSound()
    {
        if (_forwardInput >= 1)
        {
            _sound._pitch = Mathf.Lerp(_sound._pitch, 2, Time.deltaTime * 2);
            if (_initial == 0)
            {
                _sound.SoundPlay();
                _initial++;
            }
        }
        else if (_forwardInput <= 0)
        {
            _sound._pitch = Mathf.Lerp(_sound._pitch, 0.5f, Time.deltaTime * 2);
        }
    }

    public Transform GetCameraPosition()
    {
        return _cameraPosition;
    }

    public override void MoveSideways(float input)
    {
        base.MoveSideways(input);
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
        base.Precession(input);
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
