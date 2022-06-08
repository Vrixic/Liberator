using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDoorTrigger : MonoBehaviour
{
    //script that allows us to open and close the door
    [SerializeField] private HostageDoorController hostageDoorScript;

    //if a player leaves to go to the next level, close the door behind them
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && hostageDoorScript.DoorOpen)
        {
            hostageDoorScript.CloseHostageDoor();
        }
    }
}
