using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStunnedState : AIState
{
    public float stunnedTimer = 3f;
    float currentStunTime = 3f;
    void AIState.Enter(AIAgent agent)
    {
        agent.animator.SetTrigger("Flashbang");
        if(agent.transform.parent.tag == "Juggernaut")
        {
            currentStunTime = 6f;
        }
        agent.navMeshAgent.isStopped = true;
        agent.isFlashed = true;
    }

    void AIState.Exit(AIAgent agent)
    {
        agent.isFlashed = false;
        currentStunTime = stunnedTimer;
        agent.navMeshAgent.isStopped = false;
    }

    AIStateID AIState.GetId()
    {
        return AIStateID.Stunned;
    }

    void AIState.Update(AIAgent agent)
    {
        currentStunTime -= Time.deltaTime;
        if(currentStunTime <= 0.0f)
        {
            agent.stateMachine.ChangeState(AIStateID.Idle);
        }
    }
}
