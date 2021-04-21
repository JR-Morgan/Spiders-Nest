using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DoorObserver : MonoBehaviourPun
{
    #region Singleton
    public static bool IsInitialised { get; private set; } = false;
    private static DoorObserver _instance;
    public static DoorObserver Instance { get
        {
            if (!IsInitialised)
            {
                var method = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod();
                UnityEngine.Debug.LogWarning($"{typeof(DoorObserver)} was accessed before it was initialised.\nCalled from: {method.ReflectedType.Name}.{method.Name}");
            }
            return _instance;
        }
    }
    private void SetupSingleton()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            IsInitialised = true;
        }

    }
    #endregion

    private Dictionary<string, bool> _isOpen;

    [Observed]
    private Dictionary<string, bool> DoorStates {
        get => _isOpen;
        set
        {
            _isOpen = value;
            UpdateDoors();
        }
    }

    private void UpdateDoors()
    {
        foreach (DoorBehaviour door in FindObjectsOfType<DoorBehaviour>(true))
        {
            if (door.transform.parent.gameObject.activeInHierarchy && door.WallParent.wallType == WallType.Door) //If this type of wall is a doorway
            {
                var key = ToKey(door.WallParent);
                if (DoorStates.ContainsKey(key))
                {
                    door.gameObject.SetActive(DoorStates[key]);
                }
                else
                {
                    UnityEngine.Debug.LogError($"{typeof(DoorObserver)} could not find a state for {ToVector3Int(door.WallParent)}({key}). {DoorStates.Count} doors known about", this);
                }
            }
        }

    }


    private static Vector3Int ToVector3Int(WallController wallController)
    {
        if (wallController.X == 0 && wallController.Y == 0) Debug.LogError("X and Y were zero. Group was " + wallController.Group);
            
        return new Vector3Int(wallController.X, wallController.Y, wallController.Group);
    }
    /*private static int ToKey(WallController wallController)
    {
        byte[] key = new byte[]
        {
            Convert.ToByte(wallController.X),
            Convert.ToByte(wallController.Y),
            Convert.ToByte(wallController.Group),
            0,
        };
        return BitConverter.ToInt32(key, 0);
    }*/

    private static string ToKey(WallController wallController) => ToVector3Int(wallController).ToString();
    private void RegisterWallsInScene()
    {
        var doorStates = new Dictionary<string, bool>();
        foreach (DoorBehaviour door in FindObjectsOfType<DoorBehaviour>(true))
        {
            if (door.transform.parent.gameObject.activeInHierarchy) //If this type of wall is a doorway
            {
                var key = ToKey(door.WallParent);
                if (doorStates.ContainsKey(key))
                {
                    UnityEngine.Debug.LogWarning($"{typeof(WallController)} with index {ToVector3Int(door.WallParent)}({key}) was already assigned in {typeof(DoorObserver)}", this);
                }
                else
                {
                    doorStates.Add(key, door.gameObject.activeInHierarchy);
                }

            }
        }
        DoorStates = doorStates;
        UnityEngine.Debug.Log($"Registered {doorStates.Count} doors in scene",this);
        UpdateClients();
    }


    private void Awake()
    {
        SetupSingleton();
    }

    private void Start()
    {
        if (PlayerManager.IsMasterOrOffline)
        {
            RegisterWallsInScene();
        }
        else
        {
            this.photonView.RPC(nameof(SyncRequestHandler), RpcTarget.MasterClient);
        }
    }

    public void ChangeDoorState(DoorBehaviour door, bool state) => ChangeDoorState(ToKey(door.WallParent), state);
    public void ChangeDoorState(string key, bool state)
    {
        if (PlayerManager.IsMasterOrOffline)
        {
            ChangeDoorStateHandler(key, state);
        }
        else
        {
            this.photonView.RPC(nameof(ChangeDoorStateHandler), RpcTarget.MasterClient, key, state);
        }
    }

    [PunRPC]
    public void ChangeDoorStateHandler(string key, bool state)
    {
        if (PlayerManager.IsMasterOrOffline)
        {
            UnityEngine.Debug.Log($"Received request to change state of door {key}");
            DoorStates[key] = state;
            UpdateDoors();
            UpdateClients();
        }

    }

    private void UpdateClients()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //this.photonView.RPC(nameof(SyncChangeHandler), RpcTarget.Others, key, state);
            this.photonView.RPC(nameof(SyncStateHandler), RpcTarget.Others, DoorStates);
            UnityEngine.Debug.Log($"Master sent state of size : {DoorStates.Count} to all clients.");
        }
    }


    [PunRPC]
    public void SyncRequestHandler()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            this.photonView.RPC(nameof(SyncStateHandler), RpcTarget.Others, DoorStates);
        }
    }

    [PunRPC]
    public void SyncStateHandler(Dictionary<string,bool> state)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            DoorStates = state;
            UnityEngine.Debug.LogError("Client received state of size : " + state.Count);
        }
    }

    [PunRPC]
    public void SyncChangeHandler(string key, bool state)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            DoorStates[key] = state;
            UpdateDoors();
        }
    }



}
