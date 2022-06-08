using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFlashState : AIState
{
    float flashTimer = 5.0f;
    float currentFlashTimer = 5.0f;
    public AIStateID GetId()
    {
        return AIStateID.Flashed;        
    }
    public void Enter(AIAgent agent)
    {
        agent.currentState = AIStateID.Flashed;
        agent.navMeshAgent.isStopped = true;
        agent.animator.SetBool("Flashbang", true);
        agent.isFlashed = true;

    }
    public void Update(AIAgent agent)
    {
        currentFlashTimer -= Time.deltaTime;
        if (currentFlashTimer <= 0.0f) 
        {
            if (!agent.sensor.IsInsight())
            {
                agent.stateMachine.ChangeState(AIStateID.Idle);
            }
            else if (agent.sensor.IsInsight())
            {
                agent.stateMachine.ChangeState(AIStateID.AttackPlayer);
            }
        }
    }
    public void Exit(AIAgent agent)
    {
        agent.navMeshAgent.isStopped = false;
        currentFlashTimer = flashTimer;
        agent.animator.SetBool("Flashbang", false);
        agent.isFlashed = false;
    }
}
