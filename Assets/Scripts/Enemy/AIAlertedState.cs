using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAlertedState : AIState
{
    void AIState.Enter(AIAgent agent)
    {
        agent.currentState = AIStateID.Alerted;
        agent.navMeshAgent.isStopped = true;
    }

    void AIState.Exit(AIAgent agent)
    {
        agent.navMeshAgent.isStopped = false;
    }

    AIStateID AIState.GetId()
    {
        return AIStateID.Alerted;
    }

    void AIState.Update(AIAgent agent)
    {
        //have enemy look at the player from a still position
        agent.Rotating();

        float sqrDistance = (GameManager.Instance.playerTransform.position - agent.transform.position).sqrMagnitude;

        //if the player is within the enemies detection range
        if (sqrDistance <= agent.config.maxDistance)
        {
            //check if the player is in the enemies FOV
            if (agent.sensor.IsInsightWithAngleDistance())
            {
                agent.stateMachine.ChangeState(AIStateID.ChasePlayer);
            }
        }
    }
}
