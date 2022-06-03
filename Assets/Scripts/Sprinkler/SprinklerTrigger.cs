using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprinklerTrigger : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigger Entered");
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            AIAgent agent = other.gameObject.GetComponent<AIAgent>();
            if (agent.currentState != AIStateID.Stunned)
            {
                agent.stateMachine.ChangeState(AIStateID.Stunned);
            }
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameManager.Instance.playerScript.FlashPlayer();
        }
    }
}
