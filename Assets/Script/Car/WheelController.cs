using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using static UnityEngine.ParticleSystem;

public class WheelController : Vehicle, ICar, IShooter, IHitReceiver
{
    [SerializeField]
    private float _turnSpeed = 65f;
    [SerializeField]
    private float _driftAngle = 10f;
    [SerializeField]
    private float _tiltSpeed = 5f;
    [SerializeField]
    private WheelCollider _frontRight, _frontLeft, _rearRight, _rearLeft;
    [SerializeField]
    CarSound _sound;
    [SerializeField]
    private Transform _cameraPosition;
    [SerializeField]
    private Transform _firePoint;
    [SerializeField]
    private Transform _projectilePrefab;

    private Rigidbody _rb;
    private Transform _carbody;
    private float _forwardInput;
    private float _sideInput;
    private float _fireInput;
    private InputReader _inputReader;
    private PhotonView _photonView;
    private float _soundPitch = 1;
    private int _initial = 0;
    public float Speed { get; private set; }
    private ParticleController _particle;
    private void Start()
    {
        _particle = GetComponentInChildren<ParticleController>();
        _photonView = GetComponent<PhotonView>();
        if (!_photonView.IsMine)
        {
            _particle.gameObject.SetActive(false);
            return;
        }
        _carbody = transform;
        _rb = GetComponent<Rigidbody>();
        RegisterTire(); //タイヤを割り当てる
        SubscribeInput(); //入力イベントに登録
    }

    private void SubscribeInput()
    {
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

        _inputReader.OnOtherAsObservable
            .Subscribe(_ => Shoot())
            .AddTo(this);
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
        if (_forwardInput == 1)
        {
            _particle.PlayParticle();
        }
        else
        {
            _particle.StopParticle();
        }
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

    public void Shoot()
    {
        if (!_photonView.IsMine) return;
        var projectile = PhotonNetwork.Instantiate(_projectilePrefab.name, _firePoint.position, _firePoint.rotation);
        projectile.GetComponent<StraightProjectile>().SetUp(_firePoint.forward, _firePoint.transform.position, gameObject);
    }
    public override void SpeedBuff()
    {
        Debug.Log($"---{this.name} バフ呼ばれました");
        _particle.PlayColorParticle();
        base.SpeedBuff();
    }
    public override void SpeedDebuff()
    {
        Debug.Log($"---{this.name} デバフ呼ばれました");
        base.SpeedDebuff();
    }

    public void ReceiveHit(GameObject attacker, ProjectileBase projectile)
    {
        SpeedDebuff(); //自分にスピードダウン
        if (attacker.TryGetComponent<WheelController>(out var wc))
        {
            wc.SpeedBuff(); //当てた人にスピードアップ
        }
    }
}
