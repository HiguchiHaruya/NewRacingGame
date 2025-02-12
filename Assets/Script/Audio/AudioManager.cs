using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    [SerializeField] ConvertingSound _catalog;
    [SerializeField] AudioSource _sourcePrefab;
    [SerializeField] int _initialPoolCount = 5;
    private readonly Queue<AudioSource> _pool = new Queue<AudioSource>();
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(_sourcePrefab);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public async void PlayLocal(string clipId, Vector3 pos, float volume = 1)
    {
        var audioSource = GetAudioSourceFromPool();
        audioSource.gameObject.SetActive(true);
        audioSource.transform.position = pos;
        audioSource.clip = _catalog.GetClip(clipId);
        audioSource.volume = volume;
        audioSource.Play();
        await PlayFinish(audioSource);
    }
    public void PlayGlobal(string id, Vector3 pos, float volume, PhotonView view)
    {
        view.RPC(nameof(PlayGlobalRPC), RpcTarget.All, id, pos, volume);
    }
    private AudioSource GetAudioSourceFromPool()
    {
        if (_pool.Count > 0)
        {
            var src = _pool.Dequeue();
            src.gameObject.SetActive(true);
            return src;
        }
        else
        {
            return CreateNewAudioSource();
        }
    }
    private async UniTask PlayFinish(AudioSource src)
    {
        src.gameObject.SetActive(true);
        await UniTask.WaitUntil(() => !src.isPlaying);
        src.gameObject.SetActive(false);
        _pool.Enqueue(src);
    }
    private AudioSource CreateNewAudioSource()
    {
        var obj = Instantiate(_sourcePrefab, transform);
        obj.gameObject.SetActive(true);
        obj.enabled = true;
        return obj;
    }
    [PunRPC]
    private void PlayGlobalRPC(string id, Vector3 pos, float volume)
    {
        PlayLocal(id, pos, volume);
    }
}
