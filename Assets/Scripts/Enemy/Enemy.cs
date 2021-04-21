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
    private EnemyModel _model;
    [SerializeField]
    private float _health;
    [SerializeField]
    private Transform _goal;
    #endregion

    #region Properties

    [Observed]
    public float Health { get => _health; set => _health = value; }
    public float MaxHealth => EnemyModel.maxHealth;

    public Transform Goal { get => _goal; set => _goal = value; }

    /// <summary>The speed of the <see cref="NavMeshAgent"/> proportionate to its base speed</summary>
    [Observed]
    public float SpeedProportion
    {
        get => navAgent.speed / EnemyModel.movementSpeed;
        set => navAgent.speed = EnemyModel.movementSpeed * value;
    }


    #region Initialisation

    /// <summary><c>true</c> if <see cref="EnemyModel"/> has been initialised (i.e. not <c>null</c>)</summary>
    public bool IsInitialised => EnemyModel == null;

    [Observed]
    public EnemyType ModelType
    {
        get => EnemyModel.typeID;
        set 
        {
            if(EnemyModel == null || EnemyModel.typeID != value)
                EnemyModel = EnemyManager.Instance.GetModel(value);
        }
    }
    public EnemyModel EnemyModel
    {
        get => _model;
        set
        {
            _model = value;
            Initialise(_model, _model.maxHealth); //Reinitialise to new model with max health
        }
    }

    private void Initialise(EnemyModel enemyData, float health)
    {
        this._health = health;
        this.navAgent.speed = enemyData.movementSpeed;

        transform.DestroyChildren();
        Instantiate(enemyData.prefab, transform);


        float timeOfEvolve = 0f; ;

        if (PlayerManager.IsMasterOrOffline)
        {
            timeOfEvolve = UnityEngine.Random.value < EnemyModel.proababiltyToEvolve ? Time.time + enemyData.timeUntilEvolve : -1; //TODO add some small variation
        }

        agent = EnemyAgentFactory.CreateAgent(this, enemyData.typeID, timeOfEvolve);

    }

    #endregion

    #endregion

    #region Unity Methods
    public void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        if (_model != null) EnemyModel = _model;
        else ModelType = default;
    }

    public void Start()
    {
        EnemyManager.Instance.RegisterEnemy(this);
    }

    #endregion

    #region Damage
    public void AddDamage(float damage, NetworkPlayer hitBy)
    {
        _health -= damage;
        if (_health <= 0)
        {
            if (hitBy.TryGetComponent(out PlayerInventory inventory))
            {
                inventory.AddUnchecked(5 * (int)ModelType + 10);
            }
            Invoke(nameof(Die), 0.001f); //Small delay to destroy on next frame
        }
    }

    public UnityEvent<Enemy> OnDeath;
    public void Die()
    {
        EnemyManager.Instance.UnregisterEnemy(this);
        OnDeath.Invoke(this);

        Destroy(this.gameObject);
    }
    #endregion

    #region Enemy Behaviour
    private EnemyAgent agent;
    /// <summary>
    /// Updates the <see cref="Enemy"/> through <see cref=""/>
    /// </summary>
    /// <returns><c>false</c> if the <see cref="Enemy"/> was unable to update; otherwise, <c>false</c></returns>
    public bool Tick()
    {
        if (!navAgent.isOnNavMesh) return false;

        agent.Act();
        if (_goal == null) return false;
        

        return this.navAgent.SetDestination(_goal.position);
        
    }
    #endregion
}
