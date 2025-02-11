using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IHitReceiver
{
    void ReceiveHit(GameObject attacker, ProjectileBase projectile);
}