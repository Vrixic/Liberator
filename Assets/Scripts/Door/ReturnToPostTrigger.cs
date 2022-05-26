using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToPostTrigger : MonoBehaviour
{
    //Things to note for debugging:
    //remember that this will only check the tag of a gameObject with a Collider,
    //if an empty parent object is tagged it will do nothing
    //if set up correctly and you still get the error, use another variation of TryGetComponent, likely InChildreon would work best
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BasicEnemy") || other.CompareTag("MeleeUnit"))
        {
            //grab the AiAgent script from the gameObject, MAY NEED TO USE GetComponentInChildren DEPENDING ON ENEMY PREFAB STRUCTURE
            AIAgent agent;

            agent = other.GetComponentInParent<AIAgent>();

            if (agent != null)
            {
                //tell the agent to look at the trigger and start returning to their post
                //CS_ReturnToPost will set the agent's aimDirection and set their state to ReturnToPost
                agent.CS_ReturnToPost(transform.position);
            }
            else //if try get component failed then search through the children as well for an AIAgent(could become very expensive)
            {
                Debug.LogError("GameObject tagged as BasicEnemy or MeleeUnit, yet has no AIAgent component that was found, check this script for information.");
            }
        }
    }
}