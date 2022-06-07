using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostageDoorController : MonoBehaviour
{
    private Animator doorAnimator;

    private bool doorOpen = false;
    public bool DoorOpen { get { return doorOpen; } }

    private bool doorLocked = false;

    private bool isPlaying;

    private void Start()
    {
        doorAnimator = gameObject.GetComponent<Animator>();
    }

    public void OpenHostageDoor()
    {
        doorAnimator.Play("DoorOpenNew", 0, 0.0f);
        doorOpen = true;
        AudioManager.Instance.PlayAudioAtLocation(transform.position, "DoorOpen");
    }

    public void CloseHostageDoor()
    {
        if (!doorLocked)
        {
            doorAnimator.Play("DoorCloseNew", 0, 0.0f);
            doorOpen = false;
            AudioManager.Instance.PlayAudioAtLocation(transform.position, "DoorClose");
            doorLocked = true;
        }
    }
}

