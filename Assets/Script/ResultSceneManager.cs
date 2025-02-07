using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultSceneManager : MonoBehaviour
{
    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(TransitStartScene);
    }
    private void TransitStartScene()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("StartScene");
    }
}
