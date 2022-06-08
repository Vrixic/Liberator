using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLEdoor : MonoBehaviour
{
    [SerializeField] private LEdoorController levelEntryDoorScript;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            levelEntryDoorScript.CloseAndLockDoor();
        }
    }
}
