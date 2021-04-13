using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyManager : Singleton<EnemyManager>
{
    private const int INITIAL_RESERVE_SIZE = 100;

    #region Serialised Fields
    [Header("Scene References")]
    [SerializeField]
    private Transform enemyParent;
    [SerializeField]
    private LayerMask enemyLayer;

    [Header("Evolution Settings")]

    [SerializeField]
    private EnemyTypeData[] enemyData;

    [Header("Settings")]
    [Range(0f, 1f)]
    [SerializeField]
    private float updateProportion;

    [SerializeField]
    private List<Enemy> _enemies;
    private List<Enemy> reservePool;
    #endregion

    public List<Enemy> ActivePool => _enemies;

    protected override void Awake()
    {
        base.Awake();

        if(_enemies == null) _enemies = new List<Enemy>();

        //Enemy Types Validation
        {
            if (enemyData.Length < Enum.GetNames(typeof(EnemyType)).Length) Debug.LogWarning($"{nameof(enemyData)} is missing some {typeof(EnemyType)}s", this);
            for (int i = 0; i < enemyData.Length; i++)
            {
                if ((int)enemyData[i].typeID != i) Debug.LogWarning($"Mismatch between {typeof(EnemyType)} and index in {nameof(enemyData)} at index {i}\n({enemyData[i]}) is id {(int)enemyData[i].typeID}", this);
                if (enemyData[i].movementSpeed <= 0) Debug.LogWarning($"{ nameof(EnemyTypeData.movementSpeed)} at index {i}\n({enemyData[i]}) was <= zero (was this intentional?)", this);
            }
        }


        //Enemy Initialisation
        {
            EnemyAgentFactory.Initialise(GameObject.FindGameObjectsWithTag("Player"));

            int reservePoolSize = Mathf.Max(INITIAL_RESERVE_SIZE - _enemies.Count, 0);
            reservePool = new List<Enemy>(reservePoolSize);
            for(int i = 0; i < reservePoolSize; i++)
            {
                Enemy enemy = CreateNewEnemy();
                reservePool.Add(enemy);
            }
        }

        
    }


    #region Factory methods

    /// <summary>
    /// Instantiates a new <see cref="GameObject"/> with an partially initialised <see cref="Enemy"/> component
    /// </summary>
    /// <returns></returns>
    private Enemy CreateNewEnemy()
    {
        //GameObject enemyObject = Instantiate(enemyData[(int)enemyType].prefab, position, rotation, enemyParent);
        GameObject enemyObject = new GameObject(nameof(Enemy), typeof(Enemy))
        {
            layer = enemyLayer.ToLayerNumber(),
        };
        enemyObject.SetActive(false);

        Enemy enemy = enemyObject.GetComponent<Enemy>();
        enemy.OnDeath.AddListener(() => {
            if (!RetireEnemy(enemy)) Debug.LogWarning($"{typeof(Enemy)} died but could not be removed from {typeof(EnemyManager)}", enemy);
        });

        return enemy;
    }

    public Enemy GetInitialisedEnemy(Vector3 position, EnemyType type = 0) => GetInitialisedEnemy(position, Quaternion.identity, type);
    public Enemy GetInitialisedEnemy(Vector3 position, Quaternion rotation, EnemyType enemyType = 0)
    {
        Enemy enemy;
        if(reservePool.Count > 0)
        {
            enemy = reservePool[0];
        }
        else
        {
            enemy = CreateNewEnemy();
        }
        ActivateEnemy(enemy);

        enemy.transform.position = position;
        enemy.transform.rotation = rotation;

        ReinitialiseWithType(enemy, enemyType);

        enemy.gameObject.SetActive(true);
        return enemy;
    }
    #endregion


    private int _enemiesKilled = 0;

    private int EnemiesKilled
    {
        get => _enemiesKilled;
        set
        {
            _enemiesKilled = value;
            OnEnemyKilled.Invoke(_enemiesKilled);
        }
    }

    /// <summary>
    /// Adds the enemy to the <see cref="ActivePool"/>, removing it from the reserve pool if necessary
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns>false if the <paramref name="enemy"/> was already in the <see cref="ActivePool"/></returns>
    public bool ActivateEnemy(Enemy enemy)
    {
        if (ActivePool.Contains(enemy)) return false;
        if (reservePool.Contains(enemy))
        {
            reservePool.Remove(enemy);
        }
        ActivePool.Add(enemy);
        return true;
    }

    /// <summary>
    /// Removes the <paramref name="enemy"/> from the active pool and adds to the <see cref="reservePool"/>
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns>false if the <paramref name="enemy"/> was not in the <see cref="ActivePool"/></returns>
    public bool RetireEnemy(Enemy enemy)
    {
        if (ActivePool.Remove(enemy))
        {
            reservePool.Add(enemy);
            enemy.gameObject.SetActive(false);
            return true;
        }
        return false;
    }

    public UnityEvent<int> OnEnemyKilled;

    #region Updates
    private int updateOffset;
    public void Update()
    {
        int amountToUpdate = (int)(ActivePool.Count * updateProportion);
        if (ActivePool.Count > 0)
        {
            int counter = 0;
            while(counter++ != amountToUpdate && ActivePool.Count > counter)
            {
                Enemy enemy = ActivePool[(counter + updateOffset) % ActivePool.Count];
                enemy.Tick();
            }

            updateOffset += counter;
        }
    }
    #endregion


    #region Evolution


    public void ReinitialiseWithType(Enemy enemy, EnemyType enemyType)
    {
        enemy.Initialise(enemyData[(int)enemyType]);
    }

    public bool Evolve(Enemy enemy)
    {
        int targetType = (int)enemy.EnemyType.typeID + 1;
        if (targetType >= Enum.GetNames(typeof(EnemyType)).Length) return false;
        
        ReinitialiseWithType(enemy, (EnemyType)targetType);
        return true;
    }

    #endregion
}