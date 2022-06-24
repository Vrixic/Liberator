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

        //if there is a damage indicator for this enemy then this will destroy it from the screen
        GameManager.Instance.damageIndicatorSystem.FindAndDestroyIndicator(agent.transform);

        agent.miniMapLocator.Disable();
        //agent.animator.SetBool("isDead", true);

        if(!agent.transform.parent.CompareTag("Juggernaut"))
            agent.headshot.enabled = false;

        agent.animator.enabled = false;
        agent.DisableColliders();
        agent.ragdoll.ActivateRagdoll();
        //agent.animator.Play("Death");

        AudioManager.Instance.PlayAudioAtLocation(agent.transform.position, "EnemyDeath");
        AudioManager.Instance.PlayAudioAtLocation(agent.transform.position, "BulletKillEnemy");

        GameManager.Instance.CurrentCash += 50;
        GameManager.Instance.cashRewardAmount = 50;
        GameManager.Instance.StartDisplayCashCoroutine();
       
        if (GameManager.Instance.enemiesKilled.ContainsKey(agent.aiName))
        {
            GameManager.Instance.enemiesKilled[agent.aiName]++;

        }
        else
        {
            GameManager.Instance.enemiesKilled.Add(agent.aiName, 1);
        }

        agent.navMeshAgent.isStopped = true;
        agent.StartCoroutine(DisableEnemy(agent));
    }

    public void Update(AIAgent agent)
    {
    }


    public void Exit(AIAgent agent)
    {

    }

    IEnumerator DisableEnemy(AIAgent agent)
    {
        yield return new WaitForSecondsRealtime(4.0f);
        agent.navMeshAgent.enabled = false;
        agent.ragdoll.DeactivateRagdoll();
        agent.ragdoll.DisableColliders();
    }
}
