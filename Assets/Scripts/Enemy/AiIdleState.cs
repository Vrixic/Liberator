using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIIdleState : AIState
{
    public AIStateID GetId()
    {
        return AIStateID.Idle;
    }

    public void Enter(AIAgent agent)
    {
        agent.animator.SetTrigger("Idle");
        agent.animator.SetFloat("Speed", 0);
        agent.currentState = AIStateID.Idle;
        agent.navMeshAgent.isStopped = true;
        agent.sensor.StartScan();
    }

    public void Update(AIAgent agent)
    {
        if (agent.sensor.playerFoundByRaycast)
        {
            agent.stateMachine.ChangeState(AIStateID.ChasePlayer);
        }
    }

    public void Exit(AIAgent agent)
    {
        agent.navMeshAgent.isStopped = false;
        agent.sensor.playerFoundByRaycast = false;
        agent.sensor.StopScan();
        agent.sensor.StopIdleRaycast();
    }
}
