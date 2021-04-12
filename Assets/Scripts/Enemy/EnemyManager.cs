using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyManager : Singleton<EnemyManager>
{
    private const int DEFAULT_RESERVE_SIZE = 100;

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

            int reservePoolSize = Mathf.Max(DEFAULT_RESERVE_SIZE - _enemies.Count, 0);
            reservePool = new List<Enemy>(reservePoolSize);
            for(int i = 0; i < reservePoolSize; i++)
            {
                Enemy enemy = CreateNewEnemy();
                reservePool.Add(enemy);
            }
        }

        
    }

    /// <summary>
    /// Instantiates a new <see cref="GameObject"/> with an uninitialised <see cref="Enemy"/> component
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



    #region Factory methods
    public Enemy GetInitialisedEnemy(Vector3 position, EnemyType type = 0) => GetInitialisedEnemy(position, Quaternion.identity, type);
    public Enemy GetInitialisedEnemy(Vector3 position, Quaternion rotation, EnemyType enemyType = 0)
    {
        Enemy enemy;
        if(reservePool.Count > 0)
        {
            enemy = reservePool[0];
            reservePool.RemoveAt(0);
        }
        else
        {
            enemy = CreateNewEnemy();
        }

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
            OnChange.Invoke(_enemiesKilled);
        }
    }

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
    /// Tries to remove enemy active pool and add to 
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>
    public bool RetireEnemy(Enemy enemy)
    {
        if (ActivePool.Remove(enemy))
        {
            reservePool.Add(enemy);
            enemy.gameObject.SetActive(false);
            EnemiesKilled++;
            return true;
        }
        return false;
    }

    public UnityEvent<int> OnChange;


    private int offset;
    public void Update()
    {
        int amountToUpdate = (int)(ActivePool.Count * updateProportion);
        if (ActivePool.Count > 0)
        {
            int counter = 0;
            while(counter++ != amountToUpdate && ActivePool.Count > counter)
            {
                Enemy enemy = ActivePool[(counter + offset) % ActivePool.Count];
                enemy.Tick();
            }

            offset += counter;
        }
    }


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