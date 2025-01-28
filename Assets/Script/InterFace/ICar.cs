using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ICar
{
    void Precession(float input);
    void MoveSideways(float input);
    void Breake();
    void Drift();
    void ApplyCarTilt(Transform carBody, float tiltAngle, float tiltSpeed);
    void Acceleration(Rigidbody rb);
}
