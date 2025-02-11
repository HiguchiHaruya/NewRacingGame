using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using Photon.Pun;
using Cysharp.Threading.Tasks;
//’e‚ÌBaseClass
public abstract class ProjectileBase : MonoBehaviour
{
    [SerializeField] protected float _speed = 50;
    [SerializeField] protected float _lifeTime = 5f;
    protected GameObject _shooter;
    PhotonView _photonView;
    private async void Start()
    {
        // await GetPhotonView();
        await UniTask.Delay(4000);
        PhotonNetwork.Destroy(gameObject);
    }

    public virtual async void SetUp(Vector3 dir, Vector3 firePoint, GameObject shooter)
    {
        await GetPhotonView();
        _shooter = shooter;
        _photonView.RPC("StraightShoot", RpcTarget.All, dir, firePoint);
    }
    private async UniTask GetPhotonView()
    {
        var tcs = new UniTaskCompletionSource<bool>();
        if (this.TryGetComponent<PhotonView>(out var view))
        {
            _photonView = view;
            tcs.TrySetResult(true);
        }
        else
        {
            Debug.Log("PhotonView‚ª‚È‚¢");
        }
        await tcs.Task;
    }
    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (!_photonView.IsMine) return;
        if (collision.gameObject.TryGetComponent<IHitReceiver>(out var hit))
        {
            hit.ReceiveHit(_shooter, this);
        }
        PhotonNetwork.Destroy(gameObject);
    }
    [PunRPC]
    public void StraightShoot(Vector3 dir, Vector3 firePoint)
    {
        Vector3 direction = (dir != Vector3.zero) ? dir : Vector3.forward;
        LMotion
            .Create(firePoint, firePoint + direction * _speed, _lifeTime)
            .WithEase(Ease.Linear)
            .BindToPosition(this.transform)
            .AddTo(this);
    }
}

