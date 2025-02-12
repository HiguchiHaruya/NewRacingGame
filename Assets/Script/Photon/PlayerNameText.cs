using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using PlayFab;
public class PlayerNameText : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text _nameText;
    private void Start()
    {
        if (photonView.IsMine)
        {
            PlayFabClientAPI.GetAccountInfo(new PlayFab.ClientModels.GetAccountInfoRequest(),
           result =>
           {
               _nameText.text = result.AccountInfo.TitleInfo.DisplayName;
           },
           error => Debug.Log(error.ErrorMessage));
          //  _nameText.text = PhotonNetwork.NickName;
        }
    }
}
