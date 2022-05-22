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
        if(other.CompareTag("BasicEnemy") || other.CompareTag("MeleeUnit"))
        {
            //grab the AiAgent script from the gameObject, MAY NEED TO USE GetComponentInChildren DEPENDING ON ENEMY PREFAB STRUCTURE
            AIAgent agent;

            if(other.TryGetComponent<AIAgent>(out agent))
            {
                //tell the agent to look at the trigger and start returning to their post
                //CS_ReturnToPost will set the agent's aimDirection and set their state to ReturnToPost
                agent.CS_ReturnToPost(transform.position);
            }
            else //if try get component failed then search through the children as well for an AIAgent(could become very expensive)
            {
                //try to find the AIAgent component in the children of the gameObject
                agent = other.GetComponentInChildren<AIAgent>(true);
                Debug.LogWarning("Trying to find AIAgent component in children of basic or melee enemy gameObject, this should be avoided if possible as it is inefficient.");

                if(agent != null)
                {
                    Debug.LogWarning("AIAgent component found, but consider placing the tag on the enemy gameobject that contains it's collider and AIagent component");
                    agent.CS_ReturnToPost(transform.position);
                }
                else
                    Debug.LogError("GameObject tagged as BasicEnemy or MeleeUnit, yet has no AIAgent component that was found, check this script for information.");
            }
        }
    }
}
