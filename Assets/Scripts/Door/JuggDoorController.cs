using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggDoorController : MonoBehaviour
{

    private Animator doorAnimator;

    [SerializeField] private AIAgent juggernaut;

    private bool doorOpen = false;
    public bool DoorOpen { get { return doorOpen; } }

    private void Start()
    {
        doorAnimator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        if (juggernaut != null && juggernaut.currentState == AIStateID.Death)
        {
            OpenJuggernautDoor();
            enabled = false;
        }
    }

    public void OpenJuggernautDoor()
    {
        doorAnimator.Play("DoorOpenNew", 0, 0.0f);
        doorOpen = true;
    }
}
