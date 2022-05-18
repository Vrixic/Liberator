using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    [Tooltip("Distance the player can interact with objects from")]
    [SerializeField] private float interactRange = 3f;
    [Tooltip("How often to update the text prompt on the screen with a raycast")]
    [SerializeField] private float updateInteractPromptTime = 0.2f;
    private float updateInteractPromptTimer;
    private GameObject hostageSecureScreen;
    private bool securingHostage = false;
    private GameObject secureHostagePrompt;
    private GameObject doorInteractPrompt;
    private GameObject intelInteractPrompt;
    private GameObject currentInteractPrompt;
    private Image hostageProgressBarImage;

    private void Start()
    {
        hostageSecureScreen = GameManager.Instance.hostageSecured;
        secureHostagePrompt = GameManager.Instance.secureHostageText;
        doorInteractPrompt = GameManager.Instance.doorInteractText;
        intelInteractPrompt = GameManager.Instance.intelInteractText;
        hostageProgressBarImage = GameManager.Instance.hostageProgressBar.GetComponent<Image>();
        hostageProgressBarImage.fillAmount = 0;
        GameManager.Instance.hostageProgressBar.SetActive(false);


        //intitializing the interact prompt to a value so I don't need a null check condition
        currentInteractPrompt = doorInteractPrompt;

        //timer is the value that counts down, the time is what the countdown gets reset to
        updateInteractPromptTimer = updateInteractPromptTime;
    }

    private void Update()
    {
        updateInteractPromptTimer -= Time.deltaTime;

        if (updateInteractPromptTimer < 0f)
        {
            updateInteractPromptTimer = updateInteractPromptTime;
            if (Physics.Raycast(transform.position, GameManager.Instance.playerAimVector, out RaycastHit hit, interactRange))
            {
                //enters this scope if the raycast hit a collider within the interact range

                //player is looking at a door within interact range
                if (hit.collider.CompareTag("Door"))
                {
                    bool doorIsOpen = hit.collider.gameObject.GetComponent<DoorController>().DoorOpen;

                    if (doorIsOpen == false)
                        currentInteractPrompt = doorInteractPrompt;
                    else
                        currentInteractPrompt = doorInteractPrompt;

                    currentInteractPrompt.SetActive(true);
                }
                //player is looking at intel within interact range
                else if (hit.collider.CompareTag("Intel"))
                {
                    currentInteractPrompt = intelInteractPrompt;
                    currentInteractPrompt.SetActive(true);
                }
                //player is looking at a hostage within interact range
                else if (hit.collider.CompareTag("Hostage"))
                {
                    currentInteractPrompt = secureHostagePrompt;
                    currentInteractPrompt.SetActive(true);
                }
                else
                    currentInteractPrompt.SetActive(false);

            }
            else
                currentInteractPrompt.SetActive(false);
        }
        if (securingHostage)
        {
            FillHostageProgressbar();
        }
    }

    public void ProcessInteraction(bool pressOrHoldBehavior)
    {
        //get where the player is looking from the game manager
        Vector3 forward = GameManager.Instance.playerAimVector;

        //send raycast and store whatever it collides with to check and see if it is something the player can 
        //interact with by comparing the gameObject's tag
        if (Physics.Raycast(transform.position, forward, out RaycastHit hit, interactRange))
        {
            if (pressOrHoldBehavior) //press interactions go here VVVVVVVVVVVVVVVVVV
            {
                //player interacts with a door
                if (hit.collider.CompareTag("Door"))
                {
                    //access that door's doorController script
                    DoorController doorScript = hit.collider.gameObject.GetComponent<DoorController>();

                    //interact method will decide whether that specific door needs to be opened or closed
                    doorScript.Interact();

                    //play a sound
                    //TO DO----------------------------------------------------

                }
                //player interacts with a piece of intel
                else if (hit.collider.CompareTag("Intel"))
                {
                    //add currency to the player and maybe play a sound or something
                    //TO DO----------------------------------------------------
                    GameManager.Instance.CurrentCash += 200;

                    //get that instance so we can disable it
                    GameObject intelInstance = hit.collider.gameObject;
                    intelInstance.SetActive(false);
                }
            }
            else //hold interactions go here VVVVVVVVVVVVVVVVVVVVVVVV
            {
                if (hit.collider.CompareTag("Hostage"))
                {
                    //if the player pressed E on the hostage, disable their movement until they hold for enough time
                    //to secure the hostage or if they "cancel" the action (done in methods below)
                    hostageProgressBarImage.fillAmount = 0;
                    GameManager.Instance.hostageProgressBar.SetActive(true);
                    PlayerMotor.MovementEnabled = false;
                    securingHostage = true;
                }
            }

        }

    }

    public void CancelHostageSecure()
    {
        if (securingHostage)
        {
            //reenable player's movement
            PlayerMotor.MovementEnabled = true;

            //anything we want to do when the player cancels securing the hostage goes here
            GameManager.Instance.hostageProgressBar.SetActive(false);


            //break player out of causing cancel/perform events when they aren't interacting with the hostage
            securingHostage = false;
        }
    }

    public void PerformHostageSecure()
    {
        //this runs if the player successfully completed the hold interaction (should win the floor)
        if (securingHostage)
        {
            GameManager.Instance.CurrentCash += 250;

            //reenable player's movement
            PlayerMotor.MovementEnabled = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;

            GameManager.Instance.hostageProgressBar.SetActive(false);

            // Disables virtual camera so player can not look around in the pause menu
            GameManager.Instance.virtualCam.SetActive(false);

            //add code to win the level
            hostageSecureScreen.SetActive(true);
            currentInteractPrompt.SetActive(false);

            //break player out of causing cancel/perform events when they aren't interacting with the hostage
            securingHostage = false;
        }
    }
    void FillHostageProgressbar()
    {
        if (hostageProgressBarImage.fillAmount < 1)
        {
            hostageProgressBarImage.fillAmount += Time.deltaTime / 3;
        }

    }

}
