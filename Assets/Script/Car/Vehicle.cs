using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Photon.Pun;
using Cysharp.Threading.Tasks;

public class Vehicle : MonoBehaviourPunCallbacks, ICar
{
    [SerializeField]
    private float _maxTorque; //Max速度
    public float _angle; //横移動角度
    [SerializeField]
    private float _brake; //ブレーキ力
    [SerializeField]
    private float _speedDebuff = 150;
    private float _torque = 0; //現在の速度
    float steer = 0;
    protected WheelCollider frontRight, frontLeft, rearRight, rearLeft; //タイヤ達
                                                                        // private CarState _currentState;
    public float Torque => _torque;
    public static Vehicle Instance;
    public virtual  void SpeedBuff()
    {
   
    }
    public virtual async void SpeedDebuff()
    {
        frontRight.brakeTorque = _speedDebuff;
        frontLeft.brakeTorque = _speedDebuff;
        rearLeft.brakeTorque = _speedDebuff;
        rearRight.brakeTorque = _speedDebuff;
        await UniTask.Delay(1000);
        frontRight.brakeTorque = 0;
        frontLeft.brakeTorque = 0;
        rearLeft.brakeTorque = 0;
        rearRight.brakeTorque = 0;
    }
    /// <summary>前移動メソッド</summary>
    public virtual void Precession(float input)
    {
        if (input > 0)
        {
            _torque = _maxTorque;
        }
        else if (input < 0)
        {
            _torque = -1 * _maxTorque;
        }
        else if (input == 0)
        {
            _torque = 0;
        }
        rearLeft.motorTorque = _torque;
        rearRight.motorTorque = _torque;
        frontLeft.motorTorque = _torque;
        frontRight.motorTorque = _torque;
    }
    /// <summary>横移動メソッド </summary>
    public virtual void MoveSideways(float input)
    {
        //var leftInput = InputManager.Instance._inputActions.PlayerActionMap.MoveLeft.ReadValue<float>();
        //var rightInput = InputManager.Instance._inputActions.PlayerActionMap.MoveRight.ReadValue<float>();
        //if (input > 0)
        //{
        //    steer = angle * input;
        //}
        //else if (rightInput < 0)
        //{
        //    steer = angle * rightInput;
        //}
        //else
        //{
        //    steer = 0;
        //}
        frontLeft.steerAngle = steer;
        frontRight.steerAngle = steer;
    }
    public virtual void ApplyCarTilt(Transform carBody, float tiltAngle, float tiltSpeed)
    {
        float targetTilt = Input.GetAxis("Horizontal") * tiltAngle;
        Vector3 newAngle = carBody.localEulerAngles;
        newAngle.z = Mathf.LerpAngle(carBody.localEulerAngles.z, targetTilt, Time.deltaTime * tiltSpeed); //傾きをスムーズにする為にLerpを使う
        carBody.localEulerAngles = newAngle; //車体の回転を更新
    }
    public virtual void Breake()
    {
        ////  var breakeInput = InputManager.Instance._inputActions.PlayerActionMap.Brake.ReadValue<float>();
        //  float brakeforce = breakeInput > 0 ? brake : 0;
        //  frontLeft.brakeTorque = brakeforce;
        //  frontRight.brakeTorque = brakeforce;
        //  rearLeft.brakeTorque = brakeforce;
        //  rearRight.brakeTorque = brakeforce;
    }
    public virtual void Drift()
    {
        //var driftInput = InputManager.Instance._inputActions.PlayerActionMap.Drift.ReadValue<float>();
        ////Debug.Log(rearLeft.sidewaysFriction.stiffness);
        //_isDrifting = false;
        //WheelFrictionCurve sidewaysFriction = rearLeft.sidewaysFriction;
        //float forceAppPointDistance = rearLeft.forceAppPointDistance;
        //_currentStiffness = Mathf.Lerp(_currentStiffness, _targetFriction, Time.deltaTime * _driftTransitionSpeed);
        //if (driftInput > 0)
        //{
        //    _isPushDriftButton = true;
        //    _targetFriction = _driftFriction;
        //    sidewaysFriction.stiffness = _currentStiffness;
        //    if (sidewaysFriction.stiffness <= _driftFriction + 0.01) { _isDrifting = true; }
        //    forceAppPointDistance = 0.125f;
        //}
        //else
        //{
        //    forceAppPointDistance = 0.075f;
        //    _currentStiffness = _friction;
        //    sidewaysFriction.stiffness = _friction;
        //    _isPushDriftButton = false;
        //}
        //rearLeft.forceAppPointDistance = forceAppPointDistance;
        //rearRight.forceAppPointDistance = forceAppPointDistance;
        //rearLeft.sidewaysFriction = sidewaysFriction;
        //rearRight.sidewaysFriction = sidewaysFriction;
    }
    ///<summary> 加速機能メソッド</summary>
    public virtual void Acceleration(Rigidbody rb)
    {
        //_coolTime += Time.deltaTime;
        //if ((int)_coolTime >= _coolMaxTime)
        //{
        //    if (!rb.TryGetComponent<Rigidbody>(out var rigidbody)) { return; }
        //    if (Input.GetKeyDown(KeyCode.Return))
        //    {
        //        rigidbody.AddForce(-transform.forward * 20000, ForceMode.Impulse);
        //        _coolTime = 0;
        //    }
        //}
    }
    ///  <summary>現在の車の速度を返してくれる</summary>
    /// <returns>現在の車の速度(km/h)</returns>
    public float GetCurrentSpeed()
    {
        float wheelRadius = frontLeft.radius; //タイヤの半径
        float avgRpm = (frontLeft.rpm + frontRight.rpm + rearLeft.rpm + rearRight.rpm) / 4; //各タイヤのrpm(一分間の回転数)を取得して平均を得る。要するに車輪がどんだけ回転してるかが分かる
        float speed = 2 * Mathf.PI * wheelRadius * avgRpm / 60; //タイヤの回転数から車の速度(m/s)を計算する
        return speed * 3.6f; //m/sをkm/hメートル毎秒をキロメートル毎時に変換
    }
}
//    public CarState GetCurrentState()
//    {
//      //  return _currentState;
//    }
//}
//public enum CarState
//{
//    Idle,
//    Low,
//    High
//}
