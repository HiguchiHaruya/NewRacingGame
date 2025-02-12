using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertingSound : MonoBehaviour
{
    [SerializeField] List<AudioCatalog> _catalog;
    public static ConvertingSound Instance { get; private set; }
    Dictionary<string, AudioClip> _dic;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        _dic = new Dictionary<string, AudioClip>();
        foreach(var item in _catalog)
        {
            if (!_dic.ContainsKey(item.clipID))
            {
                _dic.Add(item.clipID,item.clip);
            }
        }
    }
    public AudioClip GetClip(string clipID)
    {
        if (_dic.TryGetValue(clipID, out var clip))
        {
            return clip;
        }
        Debug.Log($"{clipID} ƒNƒŠƒbƒv‚ªŒ©‚Â‚©‚ç‚ñ");
        return null;
    }
}
