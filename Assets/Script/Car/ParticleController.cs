using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : Singleton<ParticleController>
{
    [SerializeField] ParticleSystem _particle;
    public void PlayParticle() => _particle.Play();
    public void StopParticle() => _particle.Stop();
    public async void PlayColorParticle()
    {
        _particle.Stop();
        _particle.startColor = Color.yellow;
        _particle.Play();
        await UniTask.Delay(2000);
        _particle.startColor = Color.white;
    }
}
