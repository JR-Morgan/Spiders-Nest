using Photon.Pun;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using static Enemy;

/// <summary>
/// The <see cref="EnemyManager"/> singleton manages the updating of <see cref="Enemy"/> components.
/// Each <see cref="Update"/>, A specified proportion  (see <see cref="updateProportion"/>) of <see cref="Enemies"/>
/// are updated through <see cref="Enemy.Tick"/><br/>
/// </summary>
public class EnemyManager : Singleton<EnemyManager>
{
    #region Serialised Fields
    [Header("Prefab References")]
    [SerializeField]
    [Tooltip("Reference to the " + nameof(Enemy) + " prefab to be instantiated. Note, prefab must have an " + nameof(Enemy) + " " + nameof(Component))]
    private GameObject enemyPrefab;

    [Header("Scene References")]
    [SerializeField]
    [Tooltip("The layer that enemies should be apart of once registered.")]
    private LayerMask enemyLayer;

    [Header("Settings")]
    [SerializeField]
    [Tooltip("Specify evolution order of " + nameof(EnemyModel) + ".\nensure list is ordered such that element 0 is the initial state and " + nameof(EnemyType) + " matches the " + nameof(EnemyModel.typeID))]
    private EnemyModel[] enemyModels;

    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("The proportion of enemies to be updated each "+ nameof(Update) +"(). Lower values increase performance but can lead to enemies that are slow to respond.")]
    private float updateProportion;

    [Header("Read only")]
    [SerializeField]
    [Tooltip("The number of enemies killed this game.")]
    private int numberOfKills;
    public int NumberOfKills { get => numberOfKills; set => numberOfKills = value; }
    #endregion

    /// <summary>A list of all (alive) <see cref="Enemy"/> instances managed by this <see cref="EnemyManager"/></summary>
    public List<Enemy> Enemies { get; private set; }

    /// <param name="enemyType">The specified <see cref="EnemyType"/></param>
    /// <returns>The <see cref="EnemyModel"/> with the <see cref="EnemyModel.typeID"/> specified</returns>
    public EnemyModel GetModel(EnemyType enemyType) => enemyModels[(int)enemyType];

    protected override void Awake()
    {
        //Enemy Types Validation
        {
            Debug.Assert(enemyModels.Length >= Enum.GetNames(typeof(EnemyType)).Length, $"{nameof(enemyModels)} is missing some {typeof(EnemyType)}s", this);
            for (int i = 0; i < enemyModels.Length; i++)
            {
                Debug.Assert((int)enemyModels[i].typeID == i , $"Mismatch between {typeof(EnemyType)} and index in {nameof(enemyModels)} at index {i}\n({enemyModels[i]}) is id {(int)enemyModels[i].typeID}", this);
                Debug.Assert(enemyModels[i].movementSpeed > 0, $"{ nameof(EnemyModel.movementSpeed)} at index {i}\n({enemyModels[i]}) was <= zero (was this intentional?)", this);
            }
        }

        if (Enemies == null) Enemies = new List<Enemy>();
        else Enemies.Clear();

        base.Awake();

        //FindAndRegisterEnemiesInScene();
    }

    /// <summary>
    /// Clears <see cref="Enemies"/> list and searches the scene for any <see cref="Enemy"/> instances.
    /// </summary>
    private void FindAndRegisterEnemiesInScene(bool includeInactive = false)
    {
        foreach (Enemy enemy in FindObjectsOfType<Enemy>(includeInactive))
        {
            Enemies.Add(enemy);
            if (!enemy.IsInitialised) enemy.EnemyModel = enemyModels[0];
        }
    }

    /// <summary>
    /// Removes <paramref name="enemy"/> from the <see cref="EnemyManager.Enemies"/> list.
    /// </summary>
    /// <param name="enemy">The <see cref="Enemy"/> to be registered</param>
    public void RegisterEnemy(Enemy enemy)
    {
        if (Enemies.Contains(enemy)) return;

        Enemies.Add(enemy);
    }

    /// <summary>
    /// Removes the <paramref name="enemy"/> from the <see cref="EnemyManager.Enemies"/> list.
    /// </summary>
    /// <param name="enemy">The <see cref="Enemy"/> to be unregistered</param>
    /// <returns>
    /// <c>true</c> if the <paramref name="enemy"/> was successfully remove; otherwise, <c>false</c>.<br/>
    /// Also returns <c>false</c> if <paramref name="enemy"/> was not found in the <see cref="Enemies"/> list.
    /// </returns>
    public bool UnregisterEnemy(Enemy enemy) => Enemies.Remove(enemy);


    #region Factory methods


    /// <summary>
    /// Creates a new instance of <see cref="enemyPrefab"/> if the following condition is <c>true</c>.<br/>
    /// <code>!<see cref="PhotonNetwork.IsConnected"/> || <see cref="PhotonNetwork.IsMasterClient"/></code>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="enemyType"></param>
    /// <returns><c>true</c> if the <see cref="GameObject"/> was successful instantiated</returns>
    /// <remarks>
    /// Will use <see cref="PhotonNetwork"/> to instantiate if <see cref="PhotonNetwork.IsMasterClient"/>.<br/>
    /// </remarks>
    public bool CreateEnemy(out Enemy enemy, Vector3 position, Quaternion rotation, EnemyType enemyType = default)
    {
        GameObject enemyGameObject;
        if (!PhotonNetwork.IsConnected)
        {
            enemyGameObject = Instantiate(enemyPrefab, position, rotation);
        }
        else if(PhotonNetwork.IsMasterClient)
        {
            enemyGameObject = PhotonNetwork.Instantiate($"Prefabs/Enemy/{enemyPrefab.name}", position, rotation);
        }
        else
        {
            enemy = null;
            return false;
        }

        //Client side only setup (some of which gets observed and sent to other clients)
        enemyGameObject.layer = enemyLayer.ToLayerNumber();
        enemy = enemyGameObject.GetComponent<Enemy>();
        enemy.OnDeath.AddListener(e => OnEnemyDeath.Invoke(e, ++numberOfKills)); //It might be worth removing this listener on UnregisterEnemy
        enemy.ModelType = enemyType;

        return true;
    }
    public bool CreateEnemy(out Enemy enemy, Vector3 position, EnemyType enemyType = default) => CreateEnemy(out enemy, position, Quaternion.identity, enemyType);

    public bool CreateEnemy(out Enemy enemy, Vector3 position, Quaternion rotation, EnemyType enemyType, float health)
    {
        bool r = CreateEnemy(out enemy, position, rotation, enemyType);
        enemy.Health = health;

        return r;
    }

    #endregion

    #region Events
    /// <summary>
    /// <see cref="UnityEvent{Enemy,int}"/> invoked when any <see cref="Enemy.OnDeath"/> event is raised by any <see cref="Enemy"/> created by this <see cref="EnemyManager"/>.<br/>
    /// Event parameters are the <see cref="Enemy"/> that has died, and the cumulative <see cref="int"/> number of enemies that have died this session.
    /// </summary>
    public UnityEvent<Enemy, int> OnEnemyDeath;
    #endregion

    #region Updates

    /// <summary>The last index in the <see cref="Enemies"/> list that was updated</summary>
    private int updateOffset;

    public void Update()
    {
        int amountToUpdate = (int)(Enemies.Count * updateProportion);
        if (Enemies.Count > 0)
        {
            int counter = 0;
            while(counter++ != amountToUpdate && Enemies.Count > counter)
            {
                Enemy enemy = Enemies[(counter + updateOffset) % Enemies.Count];
                enemy.Tick();
            }

            updateOffset += counter;
        }
    }
    #endregion


    #region Evolution


    public void ReinitialiseWithType(Enemy enemy, EnemyType enemyType)
    {
        enemy.EnemyModel = enemyModels[(int)enemyType];
    }

    public bool Evolve(Enemy enemy)
    {
        int targetType = (int)enemy.EnemyModel.typeID + 1;
        if (targetType >= Enum.GetNames(typeof(EnemyType)).Length) return false;
        
        ReinitialiseWithType(enemy, (EnemyType)targetType);
        return true;
    }

    #endregion


    #region Serialisation

    private static string ENEMY_STATE_PATH => Application.persistentDataPath + @"/enemyState.json";

    public void DeserialiseEnemies()
    {
        try
        {
            string json = File.ReadAllText(ENEMY_STATE_PATH);
            SetupEnemies(JsonUtility.FromJson<EnemyWrapper>(json).data);
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}\n{e.StackTrace}", this);
        }
    }

    private void SetupEnemies(IEnumerable<EnemyData> data)
    {
        foreach(EnemyData enemyData in data)
        {
            CreateEnemy(out _, enemyData.position, Quaternion.identity, enemyData.modelType, enemyData.health);
        }
    }


    public void SerialiseEnemies()
    {
        string json = GetEnemyJSON();

        if (!File.Exists(ENEMY_STATE_PATH)) File.Create(ENEMY_STATE_PATH).Dispose();
        File.WriteAllText(ENEMY_STATE_PATH, json);
    }

    private string GetEnemyJSON()
    {
        EnemyData[] data = new EnemyData[Enemies.Count];
        for(int i = 0; i < Enemies.Count; i++)
        {
            data[i] = Enemies[i].GetSerialisationData();
        }
        EnemyWrapper enemyData = new EnemyWrapper() {data = data};

        return JsonUtility.ToJson(enemyData);
    }



    [Serializable]
    private struct EnemyWrapper
    {
        public EnemyData[] data;
    }
    #endregion
}