using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// The class represents the state and behaviour of an enemy
/// </summary>
[SelectionBase]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : ObservableMonoBehaviour<Enemy>
{
    #region Enemy Model


    private EnemyModel _enemyModel;
    public EnemyModel EnemyModel
    {
        get => _enemyModel;
        set => Initialise(_enemyModel);
    }

    [SerializeField]
    private EnemyType _modelType;
    [Observed]
    public EnemyType ModelType
    {
        get => _modelType;
        set
        {
            _modelType = value;
            if (RequiresReinitialisation)
                Initialise(_modelType);
        }
    }
    private bool RequiresReinitialisation => EnemyModel == null || EnemyModel.typeID != ModelType;
    /// <summary><c>true</c> if <see cref="EnemyModel"/> has been initialised (i.e. not <c>null</c>)</summary>
    public bool IsInitialised => EnemyModel == null; //TOOD not sure this is needed
    #endregion


    #region Initialisation

    public void Initialise(EnemyType modelType, bool resetHealth = false) => Initialise(EnemyManager.Instance.GetModel(modelType), resetHealth);

    private void Initialise(EnemyModel model, bool resetHealth = false)
    {
        _enemyModel = model;
        _modelType = _enemyModel.typeID;
        if (resetHealth)
        {
            this.Health = model.maxHealth;
        }

        this.NavAgent.speed = model.movementSpeed;

        transform.DestroyChildren();
        Instantiate(model.prefab, transform);


        float timeOfEvolve = 0f;

        if (PlayerManager.IsMasterOrOffline)
        {
            float time = Time.time + model.timeUntilEvolve + UnityEngine.Random.Range(-2, 2);
            timeOfEvolve = UnityEngine.Random.value < EnemyModel.proababiltyToEvolve ? time : -1;
        }

        agent = EnemyAgentFactory.CreateAgent(this, model.typeID, timeOfEvolve);

    }

    #endregion


    #region Unity Methods
    public void Awake()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        if (RequiresReinitialisation) Initialise(ModelType, resetHealth: ModelType == default);
    }

    public void Start()
    {
        EnemyManager.Instance.RegisterEnemy(this);
    }

    #endregion


    #region Attacking
    public bool IsAttacking { get; private set; }

    private void EndAttack()
    {
        IsAttacking = false;
    }

    public void StartAttacking()
    {
        if (!IsAttacking)
        {
            IsAttacking = true;
            Invoke(nameof(EndAttack), 1f);
        }
    }
    #endregion


    #region Health and Damage

    [SerializeField]
    private float _health;
    [Observed]
    public float Health { get => _health; set => _health = value; }
    public float MaxHealth => EnemyModel.maxHealth;

    public void AddDamage(float damage, PlayerState hitBy)
    {
        _health -= damage;
        if (_health <= 0)
        {
            if (this.TryGetComponent(out AudioSource a)) a.Play();

            if (hitBy.TryGetComponent(out PlayerInventory inventory))
            {
                inventory.AddUnchecked(5 * (int)ModelType + 10);
            }
            Invoke(nameof(Die), 0.0f); //Small delay to destroy on next frame
        }
    }


    public UnityEvent<Enemy> OnDeath;
    public void Die(bool invokeDeath = true)
    {
        EnemyManager.Instance.UnregisterEnemy(this);

        if(invokeDeath)
            OnDeath.Invoke(this);

        Destroy(this.gameObject);
    }
    #endregion


    #region Enemy Behaviour


    /// <summary>The speed of the <see cref="NavMeshAgent"/> proportionate to its base speed</summary>
    [Observed]
    public float SpeedProportion
    {
        get => NavAgent.speed / EnemyModel.movementSpeed;
        set => NavAgent.speed = EnemyModel.movementSpeed * value;
    }

    public NavMeshAgent NavAgent { get; private set; }
    private EnemyAgent agent;

    /// <summary>
    /// Updates the <see cref="Enemy"/> through <see cref="EnemyAgent.Act"/>
    /// </summary>
    public void Tick()
    {
        if (!NavAgent.isOnNavMesh) return;

        agent.Act();
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, PlayerManager.Instance.Local.transform.position) <= 4f)
        {
            PlayerManager.Instance.Local.Health -= EnemyModel.damage * Time.deltaTime;
        }

        Tick();
    }
    #endregion
    

    public void SetData(EnemyData data)
    {
        this.Health = data.health;
        this.transform.position = data.position;
        this.ModelType = data.modelType;
    }

    public EnemyData GetSerialisationData()
    {
        return new EnemyData()
        {
            health = this.Health,
            position = transform.position,
            modelType = this.ModelType,
        };
    }

    [Serializable]
    public struct EnemyData
    {
        public float health;
        public Vector3 position;
        public EnemyType modelType;
    }
}
