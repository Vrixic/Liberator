using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChasePlayerScript : AIState
{
    private bool reacting = false;

    public AIStateID GetId()
    {
        return AIStateID.ChasePlayer;
    }

    public void Enter(AIAgent agent)
    {
        //gun = agent.GetComponentInChildren<EnemyGun>();
        agent.currentState = AIStateID.ChasePlayer;
        agent.animator.SetBool("Chase", true);
    }

    public void Update(AIAgent agent)
    {

        //stops a lot of cost for the enemy.
        // gets the squared dist from player to enemy
        //float sqrDistance = (GameManager.Instance.playerTransform.position - agent.transform.position).sqrMagnitude;
        //tracks look rotation of the enemy.
        agent.Rotating();

        // if the player has left the range of the enemy, make the enemy idle
        //if (sqrDistance > agent.config.maxChaseDistance) // if player goes out of sight just go back to idle
        //{
            
           // agent.stateMachine.ChangeState(AIStateID.Idle);
        //}

        bool inSight = agent.sensor.IsInsight();

        if (inSight && reacting == false) // players is in sight of the enemy
        {
            agent.StartCoroutine(WaitForShotsCoroutine(agent));
            reacting = true;
        }
        else
        {
            //constantly sets move target for enemy to the player
            // only chase if player is in within the chase range and not in sight
            agent.navMeshAgent.destination = GameManager.Instance.playerTransform.position; // player within range    
        }

        //bool inSight = agent.sensor.IsInsight();

        //// checks if player is insight and the distance between them is < the maxdistance the player can see before having to move
        //if (inSight && sqrDistance < (agent.config.maxDistance * agent.config.maxDistance))
        //{
        //    //Debug.Log("Attack");
        //    agent.stateMachine.ChangeState(AIStateID.AttackPlayer);
        //}
        //else
        //{
        //    //Debug.Log("Chase");
        //    //constantly sets move target for enemy to the player
        //    agent.navMeshAgent.destination = GameManager.Instance.playerTransform.position;

        //    // if the player has left the range of the enemy, make the enemy idle
        //    if (sqrDistance > (agent.config.maxSightDistance * agent.config.maxSightDistance))
        //    {
        //        //Debug.Log("Idle");
        //        agent.stateMachine.ChangeState(AIStateID.Idle);
        //    }
        //}
    }

    public void Exit(AIAgent agent)
    {
        agent.animator.SetBool("Chase", false);
        reacting = false;
    }

    IEnumerator WaitForShotsCoroutine(AIAgent agent)
    {
        yield return new WaitForSeconds(0.3f);
        agent.stateMachine.ChangeState(AIStateID.AttackPlayer);
    }
}
