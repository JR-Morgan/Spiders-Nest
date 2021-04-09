using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyManager : Singleton<EnemyManager>
{
    private const int POOL_STEP_SIZE = 100;
    #region Prefab References
    [Header("Prefab References")]
    [SerializeField]
    private GameObject enemyPrefab;
    #endregion

    #region Scene references
    [Header("Scene References")]
    [SerializeField]
    private Transform enemyParent;
    [SerializeField]
    private LayerMask enemyLayer;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        Enemies = new List<Enemy>();

        if (enemyPrefab.GetComponent<Enemy>() == null) enemyPrefab.AddComponent<Enemy>();

        enemyPool = new List<Enemy>(POOL_STEP_SIZE);
        for(int i = 0; i < POOL_STEP_SIZE; i++)
        {
            Enemy enemy = CreateNewEnemy();
            enemy.gameObject.SetActive(false);
            enemyPool.Add(enemy);
        }

        
    }

    private Enemy CreateNewEnemy(Vector3 position = default, Quaternion rotation = default)
    {
        GameObject enemyObject = Instantiate(enemyPrefab, position, rotation, enemyParent);
        enemyObject.layer = Mathf.RoundToInt(Mathf.Log(enemyLayer.value, 2));
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        enemy.OnDeath.AddListener(() => {
            if (!RemoveEnemy(enemy)) Debug.LogWarning($"{typeof(Enemy)} died but could not be removed from {typeof(EnemyManager)}", enemy);
        });

        return enemy;
    }


    private List<Enemy> enemyPool;

    public List<Enemy> Enemies { get; set; }

    #region Factory methods
    public Enemy InstantiateEnemy(Vector3 position) => InstantiateEnemy(position, Quaternion.identity);
    public Enemy InstantiateEnemy(Vector3 position, Quaternion rotation)
    {
        Enemy enemy;
        if(enemyPool.Count > 0)
        {
            enemy = enemyPool[0];
            enemy.transform.position = position;
            enemy.transform.rotation = rotation;
            enemy.gameObject.SetActive(true);
            enemyPool.RemoveAt(0);
        }
        else
        {
            enemy = CreateNewEnemy(position, rotation);
        }
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

    public bool AddEnemy(Enemy enemy)
    {
        if (Enemies.Contains(enemy)) return false;
        if (enemyPool.Contains(enemy))
        {
            enemyPool.Remove(enemy);
        }
        Enemies.Add(enemy);
        return true;
    }

    public bool RemoveEnemy(Enemy enemy)
    {
        if (Enemies.Remove(enemy))
        {
            enemyPool.Add(enemy);
            enemy.gameObject.SetActive(false);
            EnemiesKilled++;
            return true;
        }
        return false;
    }

    public UnityEvent<int> OnChange;


    public void Update()
    {
        if(Enemies.Count > 0)
        {
            Enemy enemy = Enemies[Time.frameCount % Enemies.Count];
            enemy.CalculatePath();
        }
    }
}