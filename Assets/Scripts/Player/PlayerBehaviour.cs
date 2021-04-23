using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PhotonView), typeof(PlayerInventory))]
public class PlayerBehaviour : MonoBehaviour
{
    private const float DEFAULT_HEALTH = 100f;


    public UnityEvent OnDeath;
    public UnityEvent<float> OnHealthChange;

    [SerializeField]
    private float _health;
    public float Health {
        get => _health;
        set
        {
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

    public PhotonView PhotonView { get; private set; }
    public PlayerInventory Inventory { get; private set; }
    public bool IsLocal => PhotonView.IsMine;

    private void Awake()
    {
        PhotonView = this.GetComponent<PhotonView>();
        Inventory = this.GetComponent<PlayerInventory>();

        PlayerManager.Instance.RegisterNetworkPlayer(this);

        DontDestroyOnLoad(this.gameObject);

        if(PhotonNetwork.IsConnected)
        {
            OnDeath.AddListener(Restart);
        }
        else
        {
            OnDeath.AddListener(ShowDeathScreen);
        }
        Restart();
    }

    private void Restart()
    {
        Health = DEFAULT_HEALTH;
        transform.position = Vector3.zero;
    }

    private void ShowDeathScreen()
    {
        SceneManager.LoadScene(0);
    }

}
