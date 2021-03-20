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
    public UnityEvent OnDeath;

    [SerializeField]
    private float health = 100;

    [SerializeField]
    private Transform goal;

    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (goal == null) goal = GameObject.FindGameObjectWithTag("Start").transform;
        agent.destination = goal.position;
    }


    public void AddDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        OnDeath.Invoke();
    }
}
