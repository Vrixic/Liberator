using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIReturnState : AIState
{
    // Start is called before the first frame update
    public void Enter(AIAgent agent)
    {
        
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
        agent.navMeshAgent.destination = agent.GetInitialPosition();
    }
}
