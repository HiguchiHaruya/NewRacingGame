using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [SerializeField] InputField _nameInput;
    [SerializeField] Button _enterButton;
    private void Start()
    {
        _enterButton.onClick.AddListener(SetPlayerName);
    }
    void SetPlayerName()
    {
        string inputName = _nameInput.text.Trim();
        if (!string.IsNullOrEmpty(inputName))
        {
            PhotonNetwork.NickName = inputName;
            Debug.Log($"ÉvÉåÉCÉÑÅ[ñºÇê›íË : {inputName}");
        }
    }
}
