
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate BehaviourState EnemyBehaviour(BehaviourState state);

public static class EnemyAgentFactory
{

    private static List<Transform> players;

    internal static void Initialise(IEnumerable<Component> players)
    {
        EnemyAgentFactory.players = players.Select(x => x.transform).ToList();
    }

    public static EnemyAgent CreateAgent(Enemy enemy, EnemyType enemyType, float timeOfEvolve) => new EnemyAgent(CreateBehaviours(enemy, enemyType, timeOfEvolve));

    public static List<EnemyBehaviour> CreateBehaviours(Enemy enemy, EnemyType enemyType, float timeOfEvolve)
    {
        var behaviours = new List<EnemyBehaviour>();

        if (timeOfEvolve > 0f) behaviours.Add(Evolve(enemy, timeOfEvolve));

        behaviours.AddRange(enemyType switch
        {
            EnemyType.Small => Small(enemy),
            EnemyType.Medium => Medium(enemy),
            EnemyType.Big => Medium(enemy),
            EnemyType.Deamon => Medium(enemy),
            _ => throw new NotImplementedException($"{typeof(EnemyAgentFactory)} does not contain implementation for {typeof(EnemyType)} {enemyType}")
        });
        return behaviours;
    }


    #region Enemy Types
    private static EnemyBehaviour[] Small(Enemy enemy)
    {
        return new EnemyBehaviour[]
        {
            AttackTarget(enemy),
            TravelTowardsTarget(enemy),
        };
    }


    private static EnemyBehaviour[] Medium(Enemy enemy)
    {
        return new EnemyBehaviour[]
        {
            //DodgeAttack(enemy),
            AttackTarget(enemy),
            //FlankTarget(enemy),
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
                enemy.Goal = b.nearestTarget.target;
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
                enemy.Goal = b.nearestTarget.target;
                enemy.SpeedProportion = 1.2f;
                enemy.StartAttacking();

                b.shouldTerminate = true;
            }
            return b;
        }
    }


    private static EnemyBehaviour DodgeAttack(Enemy enemy, float distance = 1f, float healthProportion = 0.2f)
    {
        return Action;
        BehaviourState Action(BehaviourState b)
        {
            if (FindNearestTarget(enemy.transform, b)
                && b.nearestTarget.distance < distance
                && enemy.MaxHealth * healthProportion >= enemy.Health)
            {

                //enemy.Destination = //random position in the same room
                b.shouldTerminate = true;
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
