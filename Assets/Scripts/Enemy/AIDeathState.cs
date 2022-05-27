using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDeathState : AIState
{
    public Vector3 direction;
    //TO DO: Implement combat, health, and ragdoll death animation. ONce done, Death state
    //can be completed afterwards. Animators not initialized as this is just AI testing.

    public AIStateID GetId()
    {
        return AIStateID.Death;
    }

    public void Enter(AIAgent agent)
    {

        agent.currentState = AIStateID.Death;
        agent.SetIsDead(true);
        //diasbleTimer = Time.time + agent.GetDisableEnemyInterval();

        agent.animator.SetBool("isDead", true);
        agent.animator.Play("Death");
        GameManager.Instance.CurrentCash += 50;

        agent.navMeshAgent.isStopped = true;

        agent.DisableColliders();
    }

    public void Update(AIAgent agent)
    {
    }


    public void Exit(AIAgent agent)
    {

    }
}
