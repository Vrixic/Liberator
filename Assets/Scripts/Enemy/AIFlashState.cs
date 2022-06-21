using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFlashState : AIState
{
    float flashTimer = 5.0f;
    public AIStateID GetId()
    {
        return AIStateID.Flashed;        
    }
    public void Enter(AIAgent agent)
    {
        if (agent.isFlashed)
        {
            flashTimer += 1.0f;
        }
        agent.currentState = AIStateID.Flashed;
        agent.navMeshAgent.isStopped = true;
        agent.animator.SetBool("Flashbang", true);
        agent.isFlashed = true;
        flashTimer = GameManager.Instance.playerScript.equipmentTimer;
        Debug.Log(flashTimer);
    }
    public void Update(AIAgent agent)
    {
        flashTimer -= Time.deltaTime;
        if (flashTimer <= 0.0f) 
        {
            agent.stateMachine.ChangeState(AIStateID.AttackPlayer);
        }
    }
    public void Exit(AIAgent agent)
    {
        agent.navMeshAgent.isStopped = false;
        agent.animator.SetBool("Flashbang", false);
        agent.isFlashed = false;
    }
}
