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
    }

    public void Exit(AIAgent agent)
    {
        
    }

    public AIStateID GetId()
    {
        return AIStateID.Returning;
    }

    public void Update(AIAgent agent)
    {
        
    }
}
