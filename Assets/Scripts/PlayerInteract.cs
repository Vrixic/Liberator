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
    [Tooltip("How much cash the player recieves for collecting a piece of intel")]
    [SerializeField] private int intelCashReward = 200;
    [Tooltip("How much cash the player recieves for rescuing the hostage")]
    [SerializeField] private int hostageCashReward = 250;

    private float updateInteractPromptTimer;
    private bool securingHostage = false;
    private GameObject currentInteractPrompt;
    private Image hostageProgressBarImage;

    private void Start()
    {
        hostageProgressBarImage = GameManager.Instance.hostageProgressBar.GetComponent<Image>();
        hostageProgressBarImage.fillAmount = 0;
        GameManager.Instance.hostageProgressBar.SetActive(false);


        //intitializing the interact prompt to a value so I don't need a null check condition
        currentInteractPrompt = GameManager.Instance.openDoorInteractText;

        //timer is the value that counts down, the time is what the countdown gets reset to
        updateInteractPromptTimer = updateInteractPromptTime;
    }

    private void Update()
    {
        updateInteractPromptTimer -= Time.deltaTime;

        #region Display "E" to interact prompts
        if (updateInteractPromptTimer < 0f)
        {
            //reset the timer for interaction prompt updates
            updateInteractPromptTimer = updateInteractPromptTime;

            //raycast where the player is aiming to see if they are looking at an interactable item
            if (Physics.Raycast(transform.position, GameManager.Instance.playerAimVector, out RaycastHit hit, interactRange))
            {
                //enters this scope if the raycast hit a collider within the interact range

                //player is looking at a door within interact range
                if (hit.collider.CompareTag("Door"))
                {
                    bool doorIsOpen = hit.collider.gameObject.GetComponent<DoorController>().DoorOpen;

                    //set interaction text depending on whether a given door is open or closed
                    if (doorIsOpen == false)
                    {
                        //if player looks from an open door to a closed one
                        if (currentInteractPrompt == GameManager.Instance.closeDoorInteractText)
                        {
                            //deactivate the prompt for the old door
                            currentInteractPrompt.SetActive(false);
                        }

                        //set the new text to prompt the user to open the door
                        currentInteractPrompt = GameManager.Instance.openDoorInteractText;
                    }
                    else //door is open(needs to be closed)
                    {
                        //if player looks from a closed door to an open one
                        if (currentInteractPrompt == GameManager.Instance.openDoorInteractText)
                        {
                            //deactivate the prompt for the old door
                            currentInteractPrompt.SetActive(false);
                        }

                        //set the new text to prompt the user to close the door
                        currentInteractPrompt = GameManager.Instance.closeDoorInteractText;
                    }

                    //display the appropriate prompt to the player
                    currentInteractPrompt.SetActive(true);
                }
                //player is looking at intel within interact range
                else if (hit.collider.CompareTag("Intel"))
                {
                    currentInteractPrompt = GameManager.Instance.intelInteractText;
                    currentInteractPrompt.SetActive(true);
                }
                //player is looking at a hostage within interact range
                else if (hit.collider.CompareTag("Hostage"))
                {
                    currentInteractPrompt = GameManager.Instance.secureHostageText;
                    currentInteractPrompt.SetActive(true);
                }
                else //if not an interactable object
                    currentInteractPrompt.SetActive(false);
            }
            else //if no game object is within interact range
                currentInteractPrompt.SetActive(false);
        }
        #endregion

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

                    //disable current door interaction prompt since it is no longer accurate
                    currentInteractPrompt.SetActive(false);

                    //play a sound
                    //TO DO----------------------------------------------------

                }
                //player interacts with a piece of intel
                else if (hit.collider.CompareTag("Intel"))
                {
                    //play a sound
                    //TO DO----------------------------------------------------

                    GameManager.Instance.CurrentCash += intelCashReward;

                    //get that instance so we can disable it
                    GameObject intelInstance = hit.collider.gameObject;
                    intelInstance.SetActive(false);
                }
                //player interacts with an item tagged "shop"
                else if (hit.collider.CompareTag("Shop"))
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;

                    Time.timeScale = 0f;

                    // Disables virtual camera so player can not look around in game
                    GameManager.Instance.virtualCam.SetActive(false);

                    GameManager.Instance.shopCanvas.SetActive(true);
                    GameManager.Instance.buttonFuncScript.UpdateCashCountShopUi();
                }
            }
            else //hold interactions go here VVVVVVVVVVVVVVVVVVVVVVVV
            {
                if (hit.collider.CompareTag("Hostage"))
                {
                    //if the player pressed E on the hostage, disable their movement until they hold for enough time
                    //to secure the hostage or if they "cancel" the action (done in methods below)
                    GameManager.Instance.hostageProgressBar.SetActive(true);
                    PlayerMotor.MovementEnabled = false;
                    securingHostage = true;

                    //reset the progress bar when they press E on the hostage again
                    hostageProgressBarImage.fillAmount = 0;
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
            GameManager.Instance.CurrentCash += hostageCashReward;

            //reenable player's movement
            PlayerMotor.MovementEnabled = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;

            GameManager.Instance.hostageProgressBar.SetActive(false);

            // Disables virtual camera so player can not look around in the pause menu
            GameManager.Instance.virtualCam.SetActive(false);

            //add code to win the level
            GameManager.Instance.hostageSecured.SetActive(true);
            currentInteractPrompt.SetActive(false);

            //break player out of causing cancel/perform events when they aren't interacting with the hostage
            securingHostage = false;
        }
    }
    void FillHostageProgressbar()
    {
        if (hostageProgressBarImage.fillAmount < 1)
        {
            //divided by 3 because it takes 3 seconds to secure the hostage
            hostageProgressBarImage.fillAmount += Time.deltaTime / 3;
        }

    }

}
