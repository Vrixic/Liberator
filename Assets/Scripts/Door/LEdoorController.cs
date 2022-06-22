using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEdoorController : MonoBehaviour
{
    private Animator doorAnimator;

    [SerializeField] private GameObject levelToUnload;

    private bool doorOpen = false;
    public bool DoorOpen { get { return doorOpen; } }

    private bool doorLocked = false;

    private bool isPlaying;

    private void Start()
    {
        doorAnimator = gameObject.GetComponent<Animator>();
    }

    public void Interact()
    {
        isPlaying = (doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("DoorOpenNew") || doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("DoorCloseNew")) && doorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;

        if(!isPlaying)
        {
            if (doorOpen == false && doorLocked == false)
            {
                doorAnimator.Play("DoorOpenNew", 0, 0.0f);
                doorOpen = true;
                AudioManager.Instance.PlayAudioAtLocation(transform.position, "DoorOpen");
            }
            else if(doorOpen == true && doorLocked == false)
            {
                doorAnimator.Play("DoorCloseNew", 0, 0.0f);
                doorOpen = false;
                AudioManager.Instance.PlayAudioAtLocation(transform.position, "DoorClose");
            }
            else if(doorLocked)
            {
                //add door locked sound here

            }
        }
    }

    public void CloseAndLockDoor()
    {
        if(doorOpen)
        {
            doorAnimator.Play("DoorCloseNew", 0, 0.0f);
            doorOpen = false;
            AudioManager.Instance.PlayAudioAtLocation(transform.position, "DoorClose");
        }

        //unloads the previous level once the door finishes closing
        StartCoroutine(UnloadPreviousLevel());

        doorLocked = true;
        
    }

    IEnumerator UnloadPreviousLevel()
    {
        yield return new WaitForSecondsRealtime(3);

        levelToUnload.SetActive(false);
    }
}
