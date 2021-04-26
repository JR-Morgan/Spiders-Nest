
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public delegate BehaviourState EnemyBehaviour(BehaviourState state);

/// <summary>
/// Factory class for creating <see cref="EnemyAgent"/> setup with a basic Logic-base Subsumption AI
/// </summary>
public static class EnemyAgentFactory
{

    private static List<Transform> players;

    internal static void Initialise(IEnumerable<Component> players)
    {
        EnemyAgentFactory.players = players.Select(x => x.transform).ToList();
    }

    public static EnemyAgent CreateAgent(Enemy enemy, EnemyType enemyType, float timeOfEvolve) => new EnemyAgent(CreateBehaviours(enemy, enemyType, timeOfEvolve));

    /// <param name="enemy">The <see cref="Enemy"/> who's state is directly affected by the performance of the <see cref="EnemyBehaviour"/>s</param>
    /// <param name="enemyType">The starting <see cref="EnemyType"/> of the <paramref name="enemy"/> </param>
    /// <param name="timeOfEvolve">Time in seconds that this enemy will take to evolve. If >= 0; no evolution behaviour will be added</param>
    /// <returns>The list of <see cref="EnemyBehaviour"/> created for this <paramref name="enemy"/> based on the <paramref name="enemyType"/></returns>
    public static List<EnemyBehaviour> CreateBehaviours(Enemy enemy, EnemyType enemyType, float timeOfEvolve)
    {
        var behaviours = new List<EnemyBehaviour>();

        if (timeOfEvolve > 0f) behaviours.Add(Evolve(enemy, timeOfEvolve));

        behaviours.AddRange(enemyType switch
        {
            EnemyType.Small => BasicAI(enemy),
            EnemyType.Medium => BasicAI(enemy),
            EnemyType.Big => SmartAI(enemy),
            EnemyType.SuperBig => SmartAI(enemy),
            _ => throw new NotImplementedException($"{typeof(EnemyAgentFactory)} does not contain implementation for {typeof(EnemyType)} {enemyType}")
        });
        return behaviours;
    }


    #region Enemy Types
    private static EnemyBehaviour[] BasicAI(Enemy enemy)
    {
        return new EnemyBehaviour[]
        {
            AttackTarget(enemy),
            TravelTowardsTarget(enemy),
        };
    }


    private static EnemyBehaviour[] SmartAI(Enemy enemy)
    {
        return new EnemyBehaviour[]
        {
            //DodgeAttack(enemy),
            FlankTarget(enemy),
            AttackTarget(enemy),
            TravelTowardsTarget(enemy),
        };
    }
    #endregion;


    #region Helper Methods
    private static bool FindNearestTarget(Transform enemy, BehaviourState state)
    {
        if (state.nearestTarget.target == null)
        {
            state.nearestTarget = NearestTarget(enemy, players);
        }
        return state.nearestTarget.target != null;
    }

    private static (Transform, float) NearestTarget(Transform enemy, IEnumerable<Transform> targets)
    {
        Transform nearest = null;
        float distance = float.PositiveInfinity;

        foreach (Transform t in targets)
        {
            float d = Vector3.Distance(t.position, enemy.position);
            if (d < distance)
            {
                nearest = t;
                distance = d;
            }
        }
        return (nearest, distance);
    }

    #endregion


    #region Behaviours
    private static EnemyBehaviour TravelTowardsTarget(Enemy enemy)
    {
        return Action;
        BehaviourState Action(BehaviourState b)
        {
            if (FindNearestTarget(enemy.transform, b))
            {
                enemy.SpeedProportion = 1f;
                enemy.NavAgent.SetDestination(b.nearestTarget.target.position);
                b.shouldTerminate = true;

            }
            return b;
        }
    }

    private static EnemyBehaviour AttackTarget(Enemy enemy, float distance = 5f)
    {
        return Action;
        BehaviourState Action(BehaviourState b)
        {
            if (FindNearestTarget(enemy.transform, b)
                && b.nearestTarget.distance < distance * ((int)enemy.ModelType + 1))
            {
                enemy.NavAgent.SetDestination(b.nearestTarget.target.position);
                enemy.SpeedProportion = 1.2f;
                enemy.StartAttacking();
                
                b.shouldTerminate = false;
            }
            return b;
        }
    }


    private static EnemyBehaviour FlankTarget(Enemy enemy, float maxDistance = 15f, float minDistance = 6f)
    {
        return Action;
        BehaviourState Action(BehaviourState b)
        {
            if (FindNearestTarget(enemy.transform, b)
                && b.nearestTarget.distance < maxDistance
                && b.nearestTarget.distance > minDistance)
            {
                Vector2 enemyPos = new Vector2(enemy.transform.position.x, enemy.transform.position.z);
                Vector2 playerPos = new Vector2(b.nearestTarget.target.position.x, b.nearestTarget.target.position.z);

                Vector2 flankPoint = enemyPos.RotateAround(playerPos, Mathf.PI / 2f);

                enemy.NavAgent.SetDestination(new Vector3(flankPoint.x, 0, flankPoint.y));

                b.shouldTerminate = enemy.NavAgent.pathStatus == NavMeshPathStatus.PathComplete;
            }
            return b;
        }
    }

    private static EnemyBehaviour Evolve(Enemy enemy, float timeOfEvolve)
    {
        return Action;
        BehaviourState Action(BehaviourState b)
        {
            if (Time.time > timeOfEvolve)
            {
                //TODO trigger gestation period
                //enemy.Destination = //random position in the same room
                EnemyManager.Instance.Evolve(enemy);

                b.shouldTerminate = true;
            }
            return b;
        }
    }


    #endregion
}

public class BehaviourState
{
    public bool shouldTerminate = false;
    public (Transform target, float distance) nearestTarget;
}
