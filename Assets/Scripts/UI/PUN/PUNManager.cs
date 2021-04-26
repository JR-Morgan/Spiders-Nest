using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the behaviour of creating and joining <see cref="Photon"/> rooms and connection to the <see cref="Photon"/> servers
/// </summary>
public class PUNManager : MonoBehaviourPunCallbacks
{
    /// <summary>Singleton reference</summary>
    public static PUNManager Instance { get; private set; }

    [SerializeField]
    private string gameVersion = "1.0";
    [SerializeField]
    private string levelToLoad = "Level 1";

    void Awake()
    {
        //Singleton initialisation
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

    /// <summary>
    /// If <see cref="PhotonNetwork.IsConnected"/> will request to join the specified <paramref name="roomID"/>
    /// </summary>
    /// <param name="roomID">ID of room</param>
    /// <returns><c>true</c> if the room was successful joined; otherwise, <c>false</c></returns>
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

    /// <summary>
    /// If <see cref="PhotonNetwork.IsConnected"/> will request for a new room to be created with the specified <paramref name="roomID"/>
    /// </summary>
    /// <param name="roomID"></param>
    /// <returns><c>true</c> if the room was successful created; otherwise, <c>false</c></returns>
    public bool CreateNewRoom(string roomID = null)
    {
        if (PhotonNetwork.IsConnected)
        {
            if(PhotonNetwork.CreateRoom(roomID, new RoomOptions()))
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
