using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIReturnState : AIState
{

    Vector3 spawnPosition;

    // Start is called before the first frame update
    public void Enter(AIAgent agent)
    {
        spawnPosition = agent.GetInitialPosition();
        agent.navMeshAgent.destination = spawnPosition;
        agent.animator.SetBool("Returning", true);
    }

    public void Exit(AIAgent agent)
    {
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
            agent.stateMachine.ChangeState(AIStateID.AttackPlayer);
        }
        if (agent.transform.position  == spawnPosition)
        {
            agent.stateMachine.ChangeState(AIStateID.Idle);
        }
    }
}
