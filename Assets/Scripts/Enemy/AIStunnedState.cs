using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStunnedState : AIState
{
    public float stunTimer = 5f;
    void AIState.Enter(AIAgent agent)
    {
        agent.animator.SetBool("Flashbang", true);
        agent.navMeshAgent.isStopped = true;
    }

    void AIState.Exit(AIAgent agent)
    {
        agent.animator.SetBool("Flashbang", false);
        agent.navMeshAgent.isStopped = false;
    }

    AIStateID AIState.GetId()
    {
        return AIStateID.Stunned;
    }

    void AIState.Update(AIAgent agent)
    {
        stunTimer -= Time.deltaTime;
        if (stunTimer <= 0)
        {
            agent.stateMachine.ChangeState(AIStateID.Idle);
        }
    }
}
