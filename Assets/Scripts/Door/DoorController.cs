using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    //drag and drop assassin enemies in here
    [SerializeField] private List<AIAgent> ChasePlayerOnOpen = new List<AIAgent>();

    //once a door has been opened once, I don't want to keep trying to swap assassins to chase player anymore
    private bool chaseTriggeredOnce = false; 

    private Animator doorAnimator;

    private bool doorOpen = false;
    public bool DoorOpen { get { return doorOpen; } }

    private bool isPlaying;

    private void Start()
    {
        doorAnimator = gameObject.GetComponent<Animator>();
    }

    //let the door decide whether it needs to open or close, used for playerInteract
    public void Interact()
    {
        isPlaying = (doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("DoorOpenNew") || doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("DoorCloseNew")) && doorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;

        //check to see if an animation is already playing, if so we don't want to interact
        if(!isPlaying)
        {
            //neither animation is currently playing so...
            if (doorOpen == false)
            {
                doorAnimator.Play("DoorOpenNew", 0, 0.0f);
                doorOpen = true;

                //tell subscribed assassins to start chasing player(if any are attached)
                if (!chaseTriggeredOnce && ChasePlayerOnOpen.Count > 0)
                {
                    //loop through subscribed enemies(AiAgents in the list)
                    for (int i = 0; i < ChasePlayerOnOpen.Count; i++)
                    {
                        AIStateID agentState = ChasePlayerOnOpen[i].currentState;

                        //make sure this agent isn't dead or flashed or already chasing the player
                        if(agentState != AIStateID.Death && agentState != AIStateID.Flashed && agentState != AIStateID.ChasePlayer)
                        {
                            //manually swap the assassin over to the ChasePlayer state
                            ChasePlayerOnOpen[i].stateMachine.ChangeState(AIStateID.ChasePlayer);
                        }
                    }

                    //save the fact that this door has been opened once by the player
                    chaseTriggeredOnce = true;
                }
            }
            else if (doorOpen == true)
            {
                doorAnimator.Play("DoorCloseNew", 0, 0.0f);
                doorOpen = false;
            }
        }
    }

    //Explicitly try to open the door, used for OnTriggerEnter to open the door for the Assassin enemy
    public void Open()
    {
        isPlaying = (doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("DoorOpenNew") || doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("DoorCloseNew")) && doorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;

        //check to see if an animation is already playing, if so we don't want to interact
        if (!isPlaying)
        {
            //neither animation is currently playing so...
            if (doorOpen == false)
            {
                doorAnimator.Play("DoorOpenNew", 0, 0.0f);
                doorOpen = true;
            }
        }
    }

    //Explicitly try to close the door, used for OnTriggerLeave to close the door behind the Assassin enemy
    public void Close()
    {
        isPlaying = (doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("DoorOpenNew") || doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("DoorCloseNew")) && doorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;

        //check to see if an animation is already playing, if so we don't want to interact
        if (!isPlaying)
        {
            //neither animation is currently playing so...
            if (doorOpen == true)
            {
                //if door is open then close the door
                doorAnimator.Play("DoorCloseNew", 0, 0.0f);
                doorOpen = false;
            }
        }
    }
}
