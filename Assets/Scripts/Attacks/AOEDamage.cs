using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEDamage : MonoBehaviour
{
    [SerializeField]
    private float Damage = 100;

    private HashSet<Enemy> enemies = new HashSet<Enemy>();

    private void Start()
    {
        GetComponentInChildren<Renderer>(true).gameObject.SetActive(true);
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Enemy enemy))
        {
            enemies.Add(enemy);
            enemy.OnDeath.AddListener(OnDeath);
        }

        void OnDeath() => enemies.Remove(enemy);
    }
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemies.Remove(enemy);
        }
    }

    void Update()
    {
        foreach(Enemy e in enemies)
        {
            e.AddDamage(Damage * Time.deltaTime);
        }
    }
}
