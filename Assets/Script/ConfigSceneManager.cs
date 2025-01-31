using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigSceneManager : MonoBehaviour
{
    [SerializeField]
    Button _transitionToStart;
    private void Start()
    {
      //  _transitionToStart.onClick.AddListener(() => SceneTransitionManager.Instance.LoadSceneAsync("StartScene"));
    }
}
