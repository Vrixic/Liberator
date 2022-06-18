using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAlertedState : AIState
{
    void AIState.Enter(AIAgent agent)
    {
        agent.currentState = AIStateID.Alerted;
        agent.navMeshAgent.isStopped = true;
        agent.sensor.StartScan();
    }

    void AIState.Exit(AIAgent agent)
    {
        agent.navMeshAgent.isStopped = false;
        agent.sensor.playerFoundByRaycast = false;
        agent.sensor.StopScan();
        agent.sensor.StopIdleRaycast();
    }

    AIStateID AIState.GetId()
    {
        return AIStateID.Alerted;
    }

    void AIState.Update(AIAgent agent)
    {
        //have enemy look at the player from a still position
        agent.Rotating();

        if (agent.sensor.playerFoundByRaycast)
        {
            agent.stateMachine.ChangeState(AIStateID.ChasePlayer);
        }
    }
}
