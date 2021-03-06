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
        agent.animator.SetFloat("Speed", 1);
        if (agent.aiName == "Stabber" && agent.bFirstChase)
        {
            AudioManager.Instance.PlayAudioAtLocation(agent.transform.position, "MeleeEnemyYell");
        }
        agent.bFirstChase = false;
        agent.navMeshAgent.isStopped = false;
    }

    public void Update(AIAgent agent)
    {
        if (agent.isInHitReaction)
        {
            agent.navMeshAgent.isStopped = true;
            return;
        };
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

        bool inSight = agent.sensor.IsInsightAttackAndChase();

        if (inSight && reacting == false) // players is in sight of the enemy
        {
            agent.StartCoroutine(WaitForReactionTime(agent));
            reacting = true;
        }
        else if(reacting == true)
        {
              
        }
        else
        {
            //constantly sets move target for enemy to the player
            // only chase if player is in within the chase range and not in sight
            agent.navMeshAgent.destination = GameManager.Instance.playerTransform.position; // player within range  
        }
    }

    public void Exit(AIAgent agent)
    {
        agent.animator.SetBool("Chase", false);
        reacting = false;
    }

    IEnumerator WaitForReactionTime(AIAgent agent)
    {
        yield return new WaitForSeconds(0.3f);
        if(agent.currentState != AIStateID.Flashed && agent.currentState != AIStateID.Death)
            agent.stateMachine.ChangeState(AIStateID.AttackPlayer);
    }
}
