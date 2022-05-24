using UnityEngine;

public class AIAttackPlayerState : AIState
{
    public AIStateID GetId()
    {
        return AIStateID.AttackPlayer;
    }
    public void Enter(AIAgent agent)
    {
        //Debug.Log("Attack");
        agent.navMeshAgent.isStopped = true;
    }
    public void Update(AIAgent agent)
    {
        agent.Rotating();
        bool inSight = agent.sensor.IsInsight();
        if (!inSight)
        {
            //Debug.Log("fail");
            agent.stateMachine.ChangeState(AIStateID.ChasePlayer);
        }

        if (agent.IsMelee()) // if the weapon enemy is holding is melee type
        {
            if (agent.GetMeleeWeapon().Attack()) // check to see if enemy is capable of attacking right now
            {
                agent.animator.Play("Attack");
            }
        }
        else
        {
            if (agent.GetGun().ShootAtTarget(agent.playerTransform.position, agent.config.shootSprayRadius))
                agent.animator.Play("Attack");
        }
    }
    public void Exit(AIAgent agent)
    {
        agent.navMeshAgent.isStopped = false;
    }
}

