using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour
{
    #region Singleton and Unity Methods
    private static EnemyManager _instance;
    public static EnemyManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        enemies = new List<Enemy>();
    }
    #endregion


    #region Prefab references
    [Header("Prefab references")]
    [SerializeField]
    private GameObject enemyPrefab;
    #endregion

    public List<Enemy> enemies { get; set; }

    #region Factory methods
    public GameObject InstantiateEnemy(Vector3 position) => InstantiateEnemy(position, Quaternion.identity);
    public GameObject InstantiateEnemy(Vector3 position, Quaternion rotation)
    {
        GameObject enemyObject = Instantiate(enemyPrefab, position, rotation);
        Enemy enemy = enemyObject.GetComponent<Enemy>();

        enemies.Add(enemy);
        enemy.OnDeath.AddListener(() => RemoveEnemy(enemy));
        
        return enemyObject;
    }
    #endregion

    private int _enemiesKilled = 0;

    private int EnemiesKilled {
        get => _enemiesKilled;
        set
        {
            _enemiesKilled = value;
            OnChange.Invoke(_enemiesKilled);
        }
    }

    private void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
        EnemiesKilled++;
    }

    public UnityEvent<int> OnChange;

}
