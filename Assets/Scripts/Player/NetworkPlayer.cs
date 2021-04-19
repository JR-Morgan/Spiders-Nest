using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class NetworkPlayer : MonoBehaviour
{
    public PhotonView PhotonView { get; private set; }
    public bool IsLocal => PhotonView.IsMine;

    private void Awake()
    {
        PhotonView = this.GetComponent<PhotonView>();

        PlayerManager.Instance.RegisterNetworkPlayer(this);

        DontDestroyOnLoad(this.gameObject);
    }
}
