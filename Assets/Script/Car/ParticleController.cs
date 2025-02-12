using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] ParticleSystem _particle;
    PhotonView _view;
    public void PlayParticle() => _particle.Play();
    public void StopParticle() => _particle.Stop();
    private async void Start()
    {
        _particle.gameObject.SetActive(false);
        await GetPhotonViewAsync();
        if (_view.IsMine)
        {
            _particle.gameObject.SetActive(true);
        }
    }
    private async UniTask GetPhotonViewAsync()
    {
        var tcs = new UniTaskCompletionSource<bool>();
        if (this.transform.parent.parent.TryGetComponent<PhotonView>(out var view))
        {
            _view = view;
            tcs.TrySetResult(true);
        }
        await tcs.Task;
    }
    public async void PlayColorParticle()
    {
        _particle.Stop();
        _particle.startColor = Color.yellow;
        _particle.Play();
        await UniTask.Delay(2000);
        _particle.startColor = Color.white;
    }
}
