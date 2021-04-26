using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimationController : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    Enemy enemy;

    private void Awake()
    {
        this.RequireComponentInChildren(out animator);
        this.RequireComponentInParents(out agent);
        this.RequireComponentInParents(out enemy);
    }


    private void Update()
    {
        animator.SetFloat("Velocity", (agent.velocity.magnitude * 6 ) / ((int)enemy.ModelType + 1) / 3f);
        animator.SetBool("Attacking", enemy.IsAttacking);
        animator.SetBool("IsDead", enemy.Health <= 0);
    }
}
