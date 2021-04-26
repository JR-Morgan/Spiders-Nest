using Photon.Pun;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Encapsulates the players state 
/// </summary>
[RequireComponent(typeof(PhotonView), typeof(PlayerInventory))]
public class PlayerState : MonoBehaviour
{
    /// <summary>Triggered when the player is safe to serialise</summary>
    public static event Action OnSerialisationReady;

    public PhotonView PhotonView { get; private set; }
    public PlayerInventory Inventory { get; private set; }
    public Camera Camera { get; private set; }
    public bool IsLocal => PhotonView.IsMine;

    #region Health
    private const float DEFAULT_HEALTH = 100f;

    public UnityEvent OnDeath;
    public UnityEvent<float> OnHealthChange;


    [SerializeField]
    private float _health;
    public float Health {
        get => _health;
        set
        {
            if (_health > value && UnityEngine.Random.value < proababilityToPlayHitOnDamage) audioSource.Play();
            _health = value;
            if (_health <= 0)
            {
                OnDeath.Invoke();
            }
            else
            {
                OnHealthChange.Invoke(_health);
            }
        }
    }
    #endregion

    #region Audio
    private float proababilityToPlayHitOnDamage = 0.03f;
    private AudioSource audioSource;
    #endregion


    private void Awake()
    {
        PhotonView = this.GetComponent<PhotonView>();
        Inventory = this.GetComponent<PlayerInventory>();
        audioSource = this.GetComponent<AudioSource>();

        this.RequireComponentInChildren(out Camera camera);
        this.Camera = camera;

        PlayerManager.Instance.RegisterNetworkPlayer(this);


        DontDestroyOnLoad(this.gameObject);

        if(PhotonNetwork.IsConnected)
        {
            OnDeath.AddListener(Restart);
        }
        else
        {
            //TODO change to load death screen
            OnDeath.AddListener(() => LevelSwitchoverManager.LoadScene(false, 0, CursorLockMode.None, typeof(PlayerState)));
        }
        Restart();
    }

    private void Start()
    {
        OnSerialisationReady?.Invoke();
    }

    private void Restart()
    {
        Health = DEFAULT_HEALTH;
        transform.position = Vector3.zero;
    }


    #region Serialisation
    public static string PLAYER_STATE_PATH => Application.persistentDataPath + @"/playerState.json";

    public bool DeserialisePlayer()
    {
        try
        {
            string json = File.ReadAllText(PLAYER_STATE_PATH);
            SetupPlayer(JsonUtility.FromJson<PlayerData>(json));
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}\n{e.StackTrace}", this);
            return false;
        }
    }

    private void SetupPlayer(PlayerData data)
    {
        this.transform.position = data.position;
        this.transform.rotation = Quaternion.Euler(data.rotation);
        this.Inventory.Money = data.money;
        EnemyManager.Instance.NumberOfKills = data.kills;
    }

    public void SerialisePlayer()
    {
        string json = GetPlayerJSON();

        if (!File.Exists(PLAYER_STATE_PATH)) File.Create(PLAYER_STATE_PATH).Dispose();
        File.WriteAllText(PLAYER_STATE_PATH, json);
    }

    private string GetPlayerJSON()
    {
        PlayerData data = new PlayerData
        {
            position = transform.position,
            rotation = transform.rotation.eulerAngles,
            money = Inventory.Money,
            kills = EnemyManager.Instance.NumberOfKills,
        };

        return JsonUtility.ToJson(data);
    }

    [Serializable]
    private struct PlayerData
    {
        public Vector3 position;
        public Vector3 rotation;
        public float money;
        public float health;
        public int kills;
    }


    #endregion

}
