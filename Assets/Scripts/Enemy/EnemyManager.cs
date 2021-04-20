using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    [Tooltip("The layer that enemies should be apart of once registered")]
    private LayerMask enemyLayer;

    [Header("Evolution Settings")]
    [SerializeField]
    [Tooltip("Specify evolution order of " + nameof(EnemyModel) + ".\nensure list ordered such that element 0 is the initial state and " + nameof(EnemyType) + " matches the " + nameof(EnemyModel.typeID))]
    private EnemyModel[] enemyModels;

    [Header("Settings")]
    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("The proportion of enemies to be updated each "+ nameof(Update) +"(). Lower values increase performance but can lead to enemies that are slow to respond")]
    private float updateProportion;
    #endregion

    /// <summary>
    /// A list of all active (i.e. alive) <see cref="Enemy"/> instances managed by the <see cref="EnemyManager"/>
    /// </summary>
    public List<Enemy> Enemies { get; private set; }

    /// <param name="enemyType">The specified <see cref="EnemyType"/></param>
    /// <returns>The <see cref="EnemyModel"/> with the <see cref="EnemyModel.typeID"/> specified</returns>
    public EnemyModel GetModel(EnemyType enemyType) => enemyModels[(int)enemyType];

    protected override void Awake()
    {
        base.Awake();

        RegisterEnemiesInScene();

        //Enemy Types Validation
        {
            if (enemyModels.Length < Enum.GetNames(typeof(EnemyType)).Length) Debug.LogWarning($"{nameof(enemyModels)} is missing some {typeof(EnemyType)}s", this);
            for (int i = 0; i < enemyModels.Length; i++)
            {
                Debug.Assert((int)enemyModels[i].typeID == i , $"Mismatch between {typeof(EnemyType)} and index in {nameof(enemyModels)} at index {i}\n({enemyModels[i]}) is id {(int)enemyModels[i].typeID}", this);
                Debug.Assert(enemyModels[i].movementSpeed > 0, $"{ nameof(EnemyModel.movementSpeed)} at index {i}\n({enemyModels[i]}) was <= zero (was this intentional?)", this);
            }
        }
    }

    /// <summary>
    /// Clears <see cref="Enemies"/> list and searches scene for any <see cref="Enemy"/> instances.
    /// </summary>
    private void RegisterEnemiesInScene()
    {
        if (Enemies == null) Enemies = new List<Enemy>();
        else Enemies.Clear();

        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
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

    public bool CreateEnemy(Vector3 position, EnemyType enemyType = default) => CreateEnemy(position, Quaternion.identity, enemyType);
    public bool CreateEnemy(Vector3 position, Quaternion rotation, EnemyType enemyType = default)
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
            return false;
        }
        enemyGameObject.layer = enemyLayer.ToLayerNumber();
        Enemy enemy = enemyGameObject.GetComponent<Enemy>();
        enemy.ModelType = enemyType;
        return true;
    }

    #endregion



    #region Updates

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
}