using UnityEngine;

public class AIAttackPlayerState : AIState
{
    public AIStateID GetId()
    {
        return AIStateID.AttackPlayer;
    }
    public void Enter(AIAgent agent)
    {
        agent.navMeshAgent.isStopped = true;
        //agent.animator.SetTrigger("Attack");
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

        //Debug.Log("Shooting at player");
        if(agent.GetGun().ShootAtTarget(agent.playerTransform.position, agent.config.shootSprayRadius))
        {
            agent.animator.Play("Attack");
        }
    }
    public void Exit(AIAgent agent)
    {
        agent.navMeshAgent.isStopped = false;
    }
}

