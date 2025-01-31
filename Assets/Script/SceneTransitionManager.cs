using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Photon.Pun;
public class SceneTransitionManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Image _fadeImage;
    public static SceneTransitionManager Instance;
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else Destroy(gameObject);
    }
    public IEnumerator LoadSceneAll(string sceneName)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            float fadeDuration = 3.0f;
            float timer = 0;
            while (timer < fadeDuration) //フェードアウトさせる
            {
                float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
                if (_fadeImage != null) { _fadeImage.color = new Color(0, 0, 0, alpha); }
                else { Debug.Log("イメージnull"); }
                timer += Time.deltaTime;
                yield return null;
            }
            if (_fadeImage != null) { _fadeImage.color = new Color(0, 0, 0, 0); }
            else { Debug.Log("イメージnull"); }

            PhotonNetwork.LoadLevel(sceneName);
        }
    }


    //public void LoadSceneAsync(string sceneName)
    //{
    //    gameObject.SetActive(true);
    //    StartCoroutine(LoadSceneCoroutine(sceneName));
    //    gameObject.SetActive(true);
    //}
    //private IEnumerator LoadSceneCoroutine(string sceneName)
    //{
    //    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName); //シーンの非同期読み込み
    //    asyncLoad.allowSceneActivation = false; //ロード完了後に勝手にシーン移行されないようにfalseにしとく
    //    float fadeDuration = 3.0f;
    //    float timer = 0;
    //    while (timer < fadeDuration) //フェードアウトさせる
    //    {
    //        float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
    //        if (_fadeImage != null) { _fadeImage.color = new Color(0, 0, 0, alpha); }
    //        else { Debug.Log("イメージnull"); }
    //        timer += Time.deltaTime;
    //        yield return null;
    //    }
    //    while (asyncLoad.progress < 0.9f) //フェードアウト終了後にロード完了まで待つ
    //    {
    //        Debug.Log($"ロード中...{asyncLoad.progress}");
    //        yield return null;
    //    }
    //    if (_fadeImage != null) { _fadeImage.color = new Color(0, 0, 0, 0); }
    //    else { Debug.Log("イメージnull"); }
    //    asyncLoad.allowSceneActivation = true; //シーン移行
    //}
}
