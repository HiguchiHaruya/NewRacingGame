using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    [SerializeField] ConvertingSound _catalog;
    [SerializeField] AudioSource _sourcePrefab;
    [SerializeField] int _initialPoolCount = 5;
    private readonly Queue<AudioSource> _pool = new Queue<AudioSource>();
    private List<AudioSource> _activePool = new List<AudioSource>();
    PhotonView _view;
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
    public void SetVolume(float volume)
    {
        _sourcePrefab.volume = volume;
    }
    public void StopAudio()
    {
        foreach (var p in _activePool)
        {
            p.Stop();
        }
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
    public async void PlayLocal(string clipId, Vector3 pos)
    {
        var audioSource = GetAudioSourceFromPool();
        audioSource.gameObject.SetActive(true);
        audioSource.transform.position = pos;
        audioSource.clip = _catalog.GetClip(clipId);
        audioSource.Play();
        _activePool.Add(audioSource);
        await PlayFinish(audioSource);
    }
    public void PlayGlobal(string id, Vector3 pos, float volume)
    {
        _view.RPC(nameof(PlayGlobalRPC), RpcTarget.All, id, pos);
    }
    [PunRPC]
    private void PlayGlobalRPC(string id, Vector3 pos)
    {
        PlayLocal(id, pos);
    }
}
