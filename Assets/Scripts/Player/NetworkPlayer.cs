using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PlayerInventory))]
public class NetworkPlayer : MonoBehaviour
{
    public PhotonView PhotonView { get; private set; }
    public PlayerInventory Inventory { get; private set; }
    public bool IsLocal => PhotonView.IsMine;

    private void Awake()
    {
        PhotonView = this.GetComponent<PhotonView>();
        Inventory = this.GetComponent<PlayerInventory>();

        PlayerManager.Instance.RegisterNetworkPlayer(this);

        DontDestroyOnLoad(this.gameObject);
    }
}
