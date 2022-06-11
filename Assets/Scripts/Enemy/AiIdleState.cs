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
        agent.currentState = AIStateID.Idle;
        agent.navMeshAgent.isStopped = true;
    }

    public void Update(AIAgent agent)
    {

        //finds the player direction and checks to see if its magnitude is outside
        //the range of the agent sight.
        //Vector3 playerDirection = agent.playerTransform.position - agent.transform.position;

        //gets the direction the enemy is pointing.
        //Vector3 agentDirection = agent.transform.forward;

        //playerDirection.Normalize();

        //float dotProduct = Vector3.Dot(playerDirection, agentDirection);
        //if(dotProduct > 0.0f)
        //{

        //find the square distance between the player and enemy without completing the distance formula
        //with a square root since that is an expensive operation
        //float sqrDistance = (GameManager.Instance.playerTransform.position - agent.transform.position).sqrMagnitude;

        //if the player is within the enemies detection range
        //if (sqrDistance <= agent.config.maxChaseDistance)
        //{
        // Debug.Log("Player is within enemy detection range");
        //check if the player is in the enemies FOV
        //if (agent.sensor.IsInsightWithAngleDistance())
        //{
        //    //Debug.Log("Switching form idle to chase...");
        //    agent.stateMachine.ChangeState(AIStateID.ChasePlayer);
        //}
        //}
        

        //if the player is within the enemies detection range
        if (agent.sqrDistance <= agent.config.maxDistance)
        {
            //check if the player is in the enemies FOV
            if (agent.sensor.IsInsightWithAngleDistance())
            {
                agent.stateMachine.ChangeState(AIStateID.ChasePlayer);
            }
        }
    }

    public void Exit(AIAgent agent)
    {
        agent.navMeshAgent.isStopped = false;
    }
}
