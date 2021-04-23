using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{

    public Dictionary<Player, PlayerBehaviour> AllPlayers { get; private set; }

    public Player LocalPlayer { get; private set; }
    private PlayerBehaviour _offline;
    public PlayerBehaviour Local => PhotonNetwork.IsConnected? AllPlayers[LocalPlayer] : _offline;
    public Player MasterPlayer { get; private set; }
    public PlayerBehaviour Master => PhotonNetwork.IsConnected? AllPlayers[MasterPlayer] : _offline;

    public static bool IsMasterOrOffline => PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected;

    public void RegisterNetworkPlayer(PlayerBehaviour newPlayer)
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

    private void UpdatePlayerReferences(params PlayerBehaviour[] players) => UpdatePlayerReferences((IList<PlayerBehaviour>)players);
    private void UpdatePlayerReferences(IList<PlayerBehaviour> players)
    {
        EnemyAgentFactory.Initialise(players);
    }

    public void UpdateAllClients()
    {
        if (AllPlayers == null) AllPlayers = new Dictionary<Player, PlayerBehaviour>();
        else AllPlayers.Clear();


        PlayerBehaviour[] playersInScene = FindObjectsOfType<PlayerBehaviour>();
        foreach (PlayerBehaviour np in playersInScene)
        {
            Player p = np.PhotonView.Owner;
            AllPlayers.Add(p, np);

            Player previousMaster = MasterPlayer;

            if (p.IsMasterClient) MasterPlayer = p;
            if (p.IsLocal) LocalPlayer = p;

            if(previousMaster == p && MasterPlayer != p)
            {
                //We are no longer master!
                Debug.LogWarning($"Local is no longer {nameof(MasterPlayer)}. {MasterPlayer} is the new {nameof(MasterPlayer)}", this);
                //TODO potentially need to surrender observables.
            }

            if(p.IsMasterClient && p.IsLocal)
            {
                //We are now the master!
                //TOOD Add enemies to observables.
            }
        }
        UpdatePlayerReferences(playersInScene);
    }

    public void PlayerDisconnectHandler(Player left)
    {
        UpdateAllClients(); //Might need to wait till next frame here
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
