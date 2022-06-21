using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHostageDoor : MonoBehaviour
{
    [SerializeField] private HostageDoorController hostageDoorScript;

    //if player touches this trigger(should be placed a good distance beyond where they walk through the door)
    //close the door
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            hostageDoorScript.CloseHostageDoor();
        }
    }
}
