using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class ArcProjectile : ProjectileBase
{
    PhotonView _view;
    private void Start()
    {
        _view = GetComponent<PhotonView>();
    }
    public override  void SetUp(Vector3 dir, Vector3 firePoint, GameObject shooter)
    {
    }
}
