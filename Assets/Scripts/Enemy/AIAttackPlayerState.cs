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
        agent.animator.SetTrigger("Attack");
    }
    public void Update(AIAgent agent)
    {
        if (!agent.isFlashed)
        {
            agent.Rotating();
            bool inSight = agent.sensor.IsInsightAttackAndChase();
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
                Debug.Log(agent.sqrDistance);
                if (agent.sqrDistance < 3)
                {

                    if (agent.GetMeleeWeapon().Attack()) // check to see if enemy is capable of attacking right now
                    {
                        agent.animator.Play("Melee");
                    }
                }
                else if (agent.GetGun().ShootAtTarget(agent.playerTransform.position, agent.config.shootSprayRadius) && agent.sqrDistance >= 6)
                    agent.animator.Play("Attack");

            }
        }
    }
    public void Exit(AIAgent agent)
    {
        agent.navMeshAgent.isStopped = false;
    }
}

