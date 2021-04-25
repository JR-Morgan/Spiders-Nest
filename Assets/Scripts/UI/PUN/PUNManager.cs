using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PUNManager : MonoBehaviourPunCallbacks
{
    public static PUNManager Instance { get; private set; }

    [SerializeField]
    private string gameVersion = "1.0";
    [SerializeField]
    private string levelToLoad = "Level 1";

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        PhotonNetwork.AutomaticallySyncScene = true;
        DontDestroyOnLoad(this.gameObject);

        if (!ConnectToServer()) Debug.LogWarning("Failed to connect to photon server");
    }

    private void OnDestroy()
    {
        PhotonNetwork.Disconnect();
    }


    public bool JoinRoom(string roomID)
    {
        if (PhotonNetwork.IsConnected)
        {
            return PhotonNetwork.JoinRoom(roomID);
        }
        return false;
    }

    public bool ConnectToServer()
    {
        bool success = PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
        return success;
    }

    public bool CreateNewRoom(string roomName = null)
    {
        if (PhotonNetwork.IsConnected)
        {
            if(PhotonNetwork.CreateRoom(roomName, new RoomOptions()))
            {
                return true;
            }
        }
        return false;
    }

    #region PUN Callbacks
    public UnityEvent OnCreatedRoomEvent;
    public override void OnCreatedRoom()
    {
        Debug.Log($"{typeof(PUNManager)} Room Created", this);

        OnCreatedRoomEvent.Invoke();
    }

    public UnityEvent<short, string> OnCreateRoomFailedEvent;
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"{typeof(PUNManager)} Failed to create room\n{returnCode}\n{message}", this);
        OnCreateRoomFailedEvent.Invoke(returnCode, message);
    }

    public UnityEvent OnJoinedRoomEvent;
    public override void OnJoinedRoom()
    {
        Debug.Log($"{typeof(PUNManager)} Joined Room", this);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(levelToLoad);
        }
        OnJoinedRoomEvent.Invoke();
    }

    public UnityEvent<short, string> OnJoinRoomFailedEvent;
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"{typeof(PUNManager)} Joining Room Failed\n{returnCode}\n{message}", this);
        OnJoinRoomFailedEvent.Invoke(returnCode, message);
    }

    public UnityEvent OnConnectedToMasterEvent;
    public override void OnConnectedToMaster()
    {
        Debug.Log($"{typeof(PUNManager)} Connected to Master", this);
        OnConnectedToMasterEvent.Invoke();
    }

    public UnityEvent<DisconnectCause> OnDisconnectedEvent;
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"{typeof(PUNManager)} Disconnected from master\n{cause}", this);
        OnDisconnectedEvent.Invoke(cause);
    }

    public UnityEvent<Player> OnPlayerLeftRoomEvent;
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{typeof(Player)} {otherPlayer.UserId} disconnected from room", this);
        OnPlayerLeftRoomEvent.Invoke(otherPlayer);
    }
    #endregion
}
