using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightProjectile : ProjectileBase
{
    private async void Start()
    {
        await UniTask.Delay(4000);
        PhotonNetwork.Destroy(gameObject);
    }
    public override void SetUp(Vector3 dir, Vector3 firePoint, GameObject shooter)
    {
        base.SetUp(dir, firePoint, shooter);
    }
}
