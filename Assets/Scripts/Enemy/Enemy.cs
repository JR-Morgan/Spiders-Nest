using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[SelectionBase]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : ObservableMonoBehaviour<Enemy>
{
    private NavMeshAgent navAgent;


    #region Serialised Fields
    [SerializeField]
    private EnemyModel model;
    [SerializeField]
    private float _health;
    [SerializeField]
    private Transform _goal;
    #endregion

    #region Properties
    public float MaxHealth => model.maxHealth;
    [Observed]
    public float Health { get => _health; set => _health = value; }
    //[Observed]
    public EnemyModel EnemyModel { get => model; set => model = value; }

    public Transform Goal { get => _goal; set => _goal = value; }

    public float SpeedProportion
    {
        get => navAgent.speed / model.movementSpeed;
        set => navAgent.speed = model.movementSpeed * value;
    }

    #endregion

    #region Initialisation

    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        if (model != null) Initialise(model, _health);
    }

    

    public void Initialise(EnemyModel enemyData) => Initialise(enemyData, enemyData.maxHealth);
    public void Initialise(EnemyModel enemyData, float health)
    {
        this.model = enemyData;
        this._health = health;
        this.navAgent.speed = enemyData.movementSpeed;

        transform.DestroyChildren();
        Instantiate(enemyData.prefab, transform);
        

        float timeOfEvolve = 0f; ;

        if (PlayerManager.Instance.IsMaster)
        {
            timeOfEvolve = UnityEngine.Random.value < EnemyModel.proababiltyToEvolve ? Time.time + enemyData.timeUntilEvolve : - 1; //TODO add some small variation
        }

        agent = EnemyAgentFactory.CreateAgent(this, enemyData.typeID, timeOfEvolve);

    }



    #endregion

    #region Damage
    public void AddDamage(float damage)
    {
        _health -= damage;
        if (_health <= 0)
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
        if (_goal == null) return false;
        

        return this.navAgent.SetDestination(_goal.position);
        
    }
    #endregion
}
