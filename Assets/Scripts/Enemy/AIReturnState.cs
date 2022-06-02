using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIReturnState : AIState
{

    // Start is called before the first frame update
    public void Enter(AIAgent agent)
    {
        //spawnPosition = agent.GetInitialPosition();
        agent.currentState = AIStateID.Returning;
        agent.navMeshAgent.destination = agent.initialPosition;
        agent.animator.SetBool("Returning", true);
        agent.navMeshAgent.stoppingDistance = 1f;
    }

    public void Exit(AIAgent agent)
    {
        agent.navMeshAgent.stoppingDistance = 3f;
        agent.animator.SetBool("Returning", false);
    }

    public AIStateID GetId()
    {
        return AIStateID.Returning;
    }

    public void Update(AIAgent agent)
    {
        agent.transform.LookAt(agent.aimDirection);

        bool inSight = agent.sensor.IsInsight();
        if (inSight) // players is in sight of the enemy
        {
            agent.stateMachine.ChangeState(AIStateID.ChasePlayer);
        }
        if ((agent.transform.position - agent.initialPosition).sqrMagnitude <= 3)
        {
            agent.stateMachine.ChangeState(AIStateID.Alerted);
        }
    }
}
