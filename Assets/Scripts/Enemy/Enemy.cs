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
    private const float DEFAULT_MAX_HEALTH = 100f;

    public UnityEvent OnDeath;

    [SerializeField]
    private float maxHealth = DEFAULT_MAX_HEALTH;

    private float health;

    [SerializeField]
    private Transform goal;

    private NavMeshAgent agent;

    private bool isStarted = true;

    void Awake()
    {
        health = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        isStarted = true;
    }

    void Start()
    {
        if (goal == null) goal = GameObject.FindGameObjectWithTag("Start").transform;
        agent.destination = goal.position;
    }

    public void OnEnable()
    {
        if (isStarted)
        {
            Awake();
            Start();
        }
    }

    public void AddDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Invoke(nameof(Die), 0.00001f);
        }


    }
    public void Die()
    {
        OnDeath.Invoke();
    }

}
