using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages local and networked players in scene for <see cref="Photon"/> enabled multi-player
/// </summary>
public class PlayerManager : Singleton<PlayerManager>
{
    /// <summary>All players in scene</summary>
    public Dictionary<Player, PlayerState> AllPlayers { get; private set; }

    private PlayerState _offline;
    public Player LocalPlayer { get; private set; }
    public PlayerState Local => PhotonNetwork.IsConnected? AllPlayers[LocalPlayer] : _offline;
    public Player MasterPlayer { get; private set; }
    public PlayerState Master => PhotonNetwork.IsConnected? AllPlayers[MasterPlayer] : _offline;

    public static bool IsMasterOrOffline => PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected;

    public void RegisterNetworkPlayer(PlayerState newPlayer)
    {
        if (PhotonNetwork.IsConnected)
        {
            UpdateAllClients();
        }
        else
        {
            _offline = newPlayer;
            UpdatePlayerReferences(newPlayer);
        }
    }

    private void UpdatePlayerReferences(params PlayerState[] players) => UpdatePlayerReferences((IList<PlayerState>)players);
    /// <summary>
    /// Updates <see cref="Component"/>s that require a reference to all players in the scene
    /// </summary>
    /// <param name="players"></param>
    private void UpdatePlayerReferences(IList<PlayerState> players)
    {
        EnemyAgentFactory.Initialise(players);
    }

    /// <summary>
    /// Updates <see cref="AllPlayers"/> with any new players that have been added to the scene
    /// </summary>
    public void UpdateAllClients()
    {
        if (AllPlayers == null) AllPlayers = new Dictionary<Player, PlayerState>();
        else AllPlayers.Clear();


        PlayerState[] playersInScene = FindObjectsOfType<PlayerState>();
        foreach (PlayerState np in playersInScene)
        {
            Player p = np.PhotonView.Owner;
            AllPlayers.Add(p, np);

            //Player previousMaster = MasterPlayer;

            if (p.IsMasterClient) MasterPlayer = p;
            if (p.IsLocal) LocalPlayer = p;

            //if(previousMaster == p && MasterPlayer != p)
            //{
            //    //We are no longer master!
            //    Debug.LogWarning($"Local is no longer {nameof(MasterPlayer)}. {MasterPlayer} is the new {nameof(MasterPlayer)}", this);
            //    //TODO potentially need to surrender observables.
            //}

            //if(p.IsMasterClient && p.IsLocal)
            //{
            //    //We are now the master!
            //    //TOOD Add enemies to observables.
            //}
        }
        UpdatePlayerReferences(playersInScene);
    }

    public void PlayerDisconnectHandler(Player left)
    {
        if (left.IsMasterClient)
        {
            Debug.Log("Master client left the game, disconnecting from room");
            PhotonNetwork.Disconnect();
            LevelSwitchoverManager.LoadScene(false, 0, CursorLockMode.None, typeof(PlayerState));
        }
        else
        {
            UpdateAllClients(); //Might need to wait till next frame here
        }
    }

    #region Serialise Fields
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private Vector3 startLocation;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        if(LocalPlayer == null)
        {
            AddPlayer();
            
            Debug.Log($"{typeof(PlayerManager)} added a new player", this);
        }
        
    }

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PUNManager.Instance == null)
            {
                Debug.LogWarning($"{nameof(PUNManager)}.{nameof(PUNManager.Instance)} was null but {nameof(PhotonNetwork.IsConnected)} was true", this);
                return;
            }
            PUNManager.Instance.OnPlayerLeftRoomEvent.AddListener(PlayerDisconnectHandler);
        }
    }

    private GameObject AddPlayer()
    {
        GameObject playerObject;
        if (PhotonNetwork.IsConnected)
        {
            playerObject = PhotonNetwork.Instantiate($@"Prefabs\Player\{playerPrefab.name}", startLocation, Quaternion.identity);
        }
        else
        {
            playerObject = Instantiate(playerPrefab, startLocation, Quaternion.identity);
        }

        return playerObject;
    }

}
