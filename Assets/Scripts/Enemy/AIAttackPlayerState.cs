using UnityEngine;

public class AIAttackPlayerState : AIState
{
    EnemyGun gun;
    public AIStateID GetId()
    {
        return AIStateID.AttackPlayer;
    }
    public void Enter(AIAgent agent)
    {
        gun = agent.GetComponentInChildren<EnemyGun>();
        agent.navMeshAgent.isStopped = true;
        agent.animator.SetTrigger("Attack");
    }
    public void Update(AIAgent agent)
    {
        agent.Rotating();
        bool inSight = agent.sensor.IsInsight();
        if (!inSight)
        {
            // Debug.Log("Exit the shoot method");
            agent.stateMachine.ChangeState(AIStateID.ChasePlayer);
        }

        gun.ShootAtTarget(agent.playerTransform.position, agent.config.shootSprayRadius);
    }
    public void Exit(AIAgent agent)
    {
        agent.navMeshAgent.isStopped = false;
    }
}

