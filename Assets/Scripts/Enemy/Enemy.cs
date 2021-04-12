using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[SelectionBase]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    #region Serialised Fields
    [SerializeField]
    private EnemyTypeData _enemyData;
    [SerializeField]
    private float health;
    [SerializeField]
    private Transform goal;
    #endregion

    private NavMeshAgent navAgent;

    #region Properties
    public float MaxHealth => _enemyData.maxHealth;
    public float Health => health;
    public EnemyTypeData EnemyType => _enemyData;

    public float SpeedProportion
    {
        get => navAgent.speed / _enemyData.movementSpeed;
        set => navAgent.speed = _enemyData.movementSpeed * value;
    }

    public Transform Goal
    {
        get => goal;
        set => goal = value;
    }

    #endregion

    #region Initialisation

    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    public void Initialise(EnemyTypeData enemyData, float health)
    {
        this._enemyData = enemyData;
        this.health = health;
        this.navAgent.speed = enemyData.movementSpeed;

        transform.DestroyChildren();
        Instantiate(enemyData.prefab, transform);


        float timeOfEvolve = UnityEngine.Random.value < EnemyType.proababiltyToEvolve ? Time.time + enemyData.timeUntilEvolve : - 1; //TODO add some small variation
        agent = EnemyAgentFactory.CreateAgent(this, enemyData.typeID, timeOfEvolve);

    }

    public void Initialise(EnemyTypeData enemyData) => Initialise(enemyData, enemyData.maxHealth);


    public void OnEnable()
    {
        EnemyManager.Instance.ActivateEnemy(this);
    }

    public void OnDisable()
    {
        EnemyManager.Instance.RetireEnemy(this);
    }
    #endregion

    #region Damage
    public void AddDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Invoke(nameof(Die), 0.00001f);
        }
    }

    public UnityEvent OnDeath = new UnityEvent();
    public void Die()
    {
        OnDeath.Invoke();
    }
    #endregion

    #region Enemy Behaviour
    private EnemyAgent agent;
    public bool Tick()
    {
        if (!navAgent.isOnNavMesh) return false;

        agent.Act();
        if (goal == null) return false;
        
        




        return this.navAgent.SetDestination(goal.position);
        
    }
    #endregion
}
