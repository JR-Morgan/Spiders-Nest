using Photon.Pun;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Manages the state of Doors and Traps for serialisation and Photon networking
/// </summary>
public class LevelStateManager : MonoBehaviourPun
{
    #region Singleton
    public static bool IsInitialised { get; private set; } = false;
    private static LevelStateManager _instance;
    public static LevelStateManager Instance { get
        {
            if (!IsInitialised)
            {
                var method = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod();
                UnityEngine.Debug.LogWarning($"{typeof(LevelStateManager)} was accessed before it was initialised.\nCalled from: {method.ReflectedType.Name}.{method.Name}");
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

    #region Door state Networking
    //false is open
    private Dictionary<string, bool> _doorStates;

    [Observed]
    private Dictionary<string, bool> DoorStates {
        get => _doorStates;
        set
        {
            _doorStates = value;
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
                    if (!DoorStates[key])
                    {
                        foreach (RoomController room in door.WallParent.Rooms)
                        {
                            if (room.TryGetComponentInChildren(out EnemySpawner spawner)) spawner.enabled = true;
                        }
                    }
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"{typeof(LevelStateManager)} could not find a state for {ToVector3Int(door.WallParent)}({key}). {DoorStates.Count} doors known about", this);
                }
            }
        }

    }


    private static Vector3Int ToVector3Int(WallController wallController)
    {
        if (wallController.X == 0 && wallController.Y == 0) Debug.LogError("X and Y were zero. Group was " + wallController.Group);
            
        return new Vector3Int(wallController.X, wallController.Y, wallController.Group);
    }

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
                    UnityEngine.Debug.LogWarning($"{typeof(WallController)} with index {ToVector3Int(door.WallParent)}({key}) was already assigned in {typeof(LevelStateManager)}", this);
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

#endregion

    #region Serialisation
    private static string DOOR_STATE_PATH => Application.persistentDataPath + @"/doorState.json";
    private static string TRAP_STATE_PATH => Application.persistentDataPath + @"/trapsState.json";

    public bool DeserialiseLevel()
    {
        try
        {
            string djson = File.ReadAllText(DOOR_STATE_PATH);
            SetupDoors(JsonUtility.FromJson<LevelData<DoorState>>(djson).data);

            string tjson = File.ReadAllText(TRAP_STATE_PATH);
            SetupTraps(JsonUtility.FromJson<LevelData<TrapState>>(tjson).data);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}\n{e.StackTrace}", this);
            return false;
        }
    }

    private void SetupDoors(IEnumerable<DoorState> data)
    {
        if (data == null) return;
        Dictionary<string, bool> doorStates = new Dictionary<string, bool>();
        foreach(DoorState doorstate in data)
        {
            doorStates.Add(doorstate.name, doorstate.state);
        }
        this.DoorStates = doorStates;
        this.UpdateDoors();
    }

    private void SetupTraps(IEnumerable<TrapState> data)
    {
        if (data == null) return;
        foreach (TrapState s in data)
        {
            if (String.IsNullOrEmpty(s.trapType)) continue;
            string name = s.trapType.Replace("(Clone)", "");
            GameObject o = Instantiate(Resources.Load<GameObject>($@"Prefabs/Traps/{name}"));
            o.transform.position = s.position;
            o.transform.rotation = Quaternion.Euler(s.rotation);
        }
    }

    public void SerialiseLevel()
    {
        WriteToFile(DOOR_STATE_PATH, GetDoorJSON());
        WriteToFile(TRAP_STATE_PATH, GetTrapJSON());

        static void WriteToFile(string path, string json)
        {
            if (!File.Exists(path)) File.Create(path).Dispose();
            File.WriteAllText(path, json);
        }
    }

    private string GetDoorJSON()
    {
        DoorState[] data = new DoorState[DoorStates.Count];
        int i = 0;
        foreach(string name in DoorStates.Keys)
        {
            data[i] = new DoorState()
            {
                name = name,
                state = DoorStates[name],
            };
            i++;
        }

        var levelData = new LevelData<DoorState>() { data = data };

        return JsonUtility.ToJson(levelData);
    }

    private string GetTrapJSON()
    {
        BasicTrap[] traps = FindObjectsOfType<BasicTrap>();
        TrapState[] states = new TrapState[traps.Length];
        int i = 0;
        foreach (BasicTrap trap in traps)
        {
            states[i] = new TrapState()
            {
                position = trap.transform.position,
                rotation = trap.transform.rotation.eulerAngles,
                trapType = trap.gameObject.name,
            };
            i++;
        }

        var levelData = new LevelData<TrapState>() { data = states };

        return JsonUtility.ToJson(levelData);
    }

    [Serializable]
    private struct LevelData<T>
    {
        public T[] data;
    }

    [Serializable]
    private struct DoorState
    {
        public string name;
        public bool state;
    }

    [Serializable]
    private struct TrapState
    {
        public string trapType;
        public Vector3 position;
        public Vector3 rotation;
    }

    #endregion


}
