using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEDamage : MonoBehaviour
{
    [SerializeField]
    private float _damage;

    public float Damage
    {
        get => _damage;
        set
        {
            _damage = value;
            enabled = _damage > 0;
        }
    }

    private HashSet<Enemy> enemies = new HashSet<Enemy>();

    private void Start()
    {
        GetComponentInChildren<Renderer>(true).gameObject.SetActive(true);
        //Damage = Damage; //Just to check if > 0
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponentInParents(out Enemy enemy))
        {
            enemies.Add(enemy);
            enemy.OnDeath.AddListener(OnDeath);
        }

        void OnDeath(Enemy e) => enemies.Remove(e);
    }
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponentInParents(out Enemy enemy))
        {
            enemies.Remove(enemy);
        }
    }

    void Update()
    {
        foreach(Enemy e in enemies)
        {
            e.AddDamage(Damage * Time.deltaTime, PlayerManager.Instance.Local);
        }
    }
}
