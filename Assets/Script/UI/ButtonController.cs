using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitMotion;
using TMPro;
using UnityEngine.EventSystems;
using LitMotion.Extensions;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class ButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("Option")]
    [SerializeField] private bool _isTransitionScene = false;
    [SerializeField] private bool _isHavePhotonView = false;
    [SerializeField] private string _sceneName;
    [Header("Components")]
    [SerializeField] private Image _fillImage;
    [SerializeField] private TMP_Text _labelText;
    [Header("Setting")]
    [SerializeField] private Color _hoverLabelColor;
    [SerializeField] private Ease _hoverEase;
    [SerializeField] private Ease _clickEase;
    [SerializeField] private float _hoverDuration;
    [SerializeField] private float _clickDuration;
    private readonly CompositeMotionHandle _handle = new(2);
    private Color _defaltColor;
    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(ButtonAnimation);
        _defaltColor = _labelText.color;
    }
    private void OnDestroy()
    {
        _handle.Cancel();
    }
    private void ButtonAnimation()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _handle.Cancel();
        LMotion.Create(Vector3.zero,Vector3.one,_hoverDuration)
            .WithEase(_hoverEase)
            .BindToLocalScale(_fillImage.transform)
            .AddTo(_handle);

        LMotion.Create(_labelText.color, _hoverLabelColor, _hoverDuration)
            .WithEase(_hoverEase)
            .BindToColor(_labelText)
            .AddTo(_handle);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _handle.Cancel();
        LMotion.Create(Vector3.one, Vector3.zero, _hoverDuration)
            .WithEase(_hoverEase)
            .BindToLocalScale(_fillImage.transform)
            .AddTo(_handle);

        LMotion.Create(_labelText.color, _defaltColor, _hoverDuration)
            .WithEase(_hoverEase)
            .BindToColor(_labelText)
            .AddTo(_handle);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        LMotion.Create(Vector3.one, Vector3.one * 0.9f, _clickDuration)
          .WithLoops(2, LoopType.Yoyo)
          .WithEase(_clickEase)
          .BindToLocalScale(transform)
          .AddTo(gameObject);

        if (!_isTransitionScene) return;
        if (_isHavePhotonView)
        {
            if(!PhotonNetwork.IsMasterClient)return;
            PhotonNetwork.LoadLevel(_sceneName);
        }
        else
        {
            SceneManager.LoadScene(_sceneName);
        }
    }
}
