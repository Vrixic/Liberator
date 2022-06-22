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
            if (other.gameObject.TryGetComponent<AIAgent>(out agent))
            {
                //Debug.Log(agent.currentState);
                //Debug.Log(agent);
                agent.stateMachine.ChangeState(AIStateID.Flashed);
                agent.currentState = AIStateID.Flashed;
            }
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameManager.Instance.playerScript.FlashPlayer();
        }
    }
}
