using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    //script that allows us to open and close the door
    [SerializeField] private DoorController doorScript;

    //if a gameObject enters the DoorOpenCloseTrigger
    private void OnTriggerEnter(Collider other)
    {
        //if it's an assassin enemy
        if(other.CompareTag("Assassin"))
        {
            doorScript.Open();
        }
    }

    //if a gameObject leaves the DoorOpenCloseTrigger
    private void OnTriggerExit(Collider other)
    {
        //if it's an assassin enemy
        if(other.CompareTag("Assassin"))
        {
            doorScript.Close();
        }
    }
}
