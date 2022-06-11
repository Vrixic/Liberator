using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprinklerTrigger : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            AIAgent agent;
            Debug.Log("Trigger Entered");
            if (other.gameObject.TryGetComponent<AIAgent>(out agent))
            {
                //Debug.Log(agent.currentState);
                //Debug.Log(agent);
                if (agent.currentState != AIStateID.Flashed)
                {
                    agent.stateMachine.ChangeState(AIStateID.Flashed);
                    Debug.Log(agent.currentState);
                }
            }
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameManager.Instance.playerScript.FlashPlayer();
        }
    }
}
