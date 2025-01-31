using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField]
    Button _transitionToConfigButton;
    [SerializeField]
    string _transitionToConfigText;
    [SerializeField]
    Button _transitionToGameButton;
    [SerializeField]
    string _transitionToGameText;
    [SerializeField]
    private InputField _playerName;
    private void Start()
    {
        //_transitionToConfigButton
        //    .onClick
        //    .AddListener(() => SceneTransitionManager.Instance.LoadSceneAsync(_transitionToConfigText));

        //_transitionToGameButton
        //    .onClick
        //    .AddListener(() => SceneTransitionManager.Instance.LoadSceneAsync(_transitionToGameText));
    }
}
