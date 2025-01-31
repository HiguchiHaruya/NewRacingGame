using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
public class PlayerNameText : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text _nameText;
    private void Start()
    {
        if (photonView.IsMine)
        {
            _nameText.text = PhotonNetwork.NickName;
        }
    }
}
