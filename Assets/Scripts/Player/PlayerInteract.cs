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

    private LayerMask interactionLayerMask;
    private float updateInteractPromptTimer;
    private bool securingHostage = false;
    public GameObject currentInteractPrompt;
    private Image hostageProgressBarImage;
    private Hostage currentHostage;
    Collider previousHit = null;

    private void Start()
    {
        hostageProgressBarImage = GameManager.Instance.hostageProgressBar.GetComponent<Image>();
        hostageProgressBarImage.fillAmount = 0;
        GameManager.Instance.hostageProgressBar.SetActive(false);
        interactionLayerMask = LayerMask.GetMask("Interaction");

        //intitializing the interact prompt to a value so I don't need a null check condition
        currentInteractPrompt = GameManager.Instance.openDoorInteractText;

        //timer is the value that counts down, the time is what the countdown gets reset to
        updateInteractPromptTimer = updateInteractPromptTime;
    }

    private void Update()
    {
        updateInteractPromptTimer -= Time.deltaTime;

        if (Time.timeScale <= 0.1f)
        {
            currentInteractPrompt.SetActive(false);
            return;
        }

        #region Display "E" to interact prompts
        if (updateInteractPromptTimer < 0f)
        {
            //reset the timer for interaction prompt updates
            updateInteractPromptTimer = updateInteractPromptTime;

            RaycastHit currentHit;
            Vector3 launchPosition = transform.position;
            launchPosition.y = GameManager.Instance.mainCamera.transform.position.y;
            //raycast where the player is aiming to see if they are looking at an interactable item
            if (Physics.Raycast(launchPosition, GameManager.Instance.playerAimVector, out currentHit, interactRange, interactionLayerMask))
            {
                if (previousHit != null)
                {                 //enters this scope if the raycast hit a collider within the interact range
                    if (previousHit != currentHit.collider)
                    {
                        currentInteractPrompt.SetActive(false);
                    }
                }
                //player is looking at a door within interact range
                if (currentHit.collider.CompareTag("Door"))
                {
                    DoorController doorController = currentHit.collider.gameObject.GetComponent<DoorController>();
                    bool doorIsOpen = false;

                    if(doorController != null)
                    {
                        doorIsOpen = doorController.DoorOpen;
                    }
                    else
                    {
                        LEdoorController levelEntryDoorController = currentHit.collider.gameObject.GetComponent<LEdoorController>();

                        if (levelEntryDoorController != null)
                            doorIsOpen = levelEntryDoorController.DoorOpen;
                        else
                        {
                            Debug.LogError("This door doesn't have a compatible door controller");
                        }
                    }

                    //set interaction text depending on whether a given door is open or closed
                    if (doorIsOpen == false)
                    {
                        //set the new text to prompt the user to open the door
                        currentInteractPrompt = GameManager.Instance.openDoorInteractText;
                    }
                    else //door is open(needs to be closed)
                    {
                        //set the new text to prompt the user to close the door
                        currentInteractPrompt = GameManager.Instance.closeDoorInteractText;
                    }

                    //display the appropriate prompt to the player
                    currentInteractPrompt.SetActive(true);
                }
                //player is looking at intel within interact range
                else if (currentHit.collider.CompareTag("Intel"))
                {
                    currentInteractPrompt = GameManager.Instance.intelInteractText;
                    currentInteractPrompt.SetActive(true);
                }
                //player is looking at a hostage within interact range
                else if (currentHit.collider.CompareTag("Hostage"))
                {
                    currentInteractPrompt = GameManager.Instance.secureHostageText;
                    currentInteractPrompt.SetActive(true);
                }
                else if(currentHit.collider.CompareTag("Shop"))
                {
                    currentInteractPrompt = GameManager.Instance.openShopInteractText;

                    if (GameManager.Instance.playerScript.GetCurrentEquippedWeapon().CanSwitchWeapon() && !GameManager.Instance.shopCanvas.activeInHierarchy)
                        currentInteractPrompt.SetActive(true);
                    else
                        currentInteractPrompt.SetActive(false);
                }
                else //if not an interactable object
                    currentInteractPrompt.SetActive(false);

                //store the previous collider hit by the interaction raycast
                previousHit = currentHit.collider;
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
        //send raycast and store whatever it collides with to check and see if it is something the player can 
        //interact with by comparing the gameObject's tag
        if (Physics.Raycast(GameManager.Instance.mainCamera.transform.position, GameManager.Instance.playerAimVector, out RaycastHit hit, interactRange, interactionLayerMask))
        {
            if (!GameManager.Instance.isPauseMenuOpen)
            {
                if (pressOrHoldBehavior) //press interactions go here VVVVVVVVVVVVVVVVVV
                {
                    //player interacts with a door
                    if (hit.collider.CompareTag("Door"))
                    {
                        //access that door's doorController script
                        DoorController doorScript = hit.collider.gameObject.GetComponent<DoorController>();

                        if (doorScript != null)
                        {
                            //interact method will decide whether that specific door needs to be opened or closed
                            doorScript.Interact();

                            //disable current door interaction prompt since it is no longer accurate
                            currentInteractPrompt.SetActive(false);
                        }
                        else
                        {
                            LEdoorController levelEntryDoorScript = hit.collider.gameObject.GetComponent<LEdoorController>();

                            levelEntryDoorScript.Interact();

                            currentInteractPrompt.SetActive(false);
                        }
                    }
                    //player interacts with a piece of intel
                    else if (hit.collider.CompareTag("Intel"))
                    {
                        GameManager.Instance.CurrentCash += intelCashReward;
                        GameManager.Instance.cashRewardAmount = intelCashReward;
                        //get that instance so we can disable it
                        GameObject intelInstance = hit.collider.gameObject;

                        intelInstance.SetActive(false);
                        AudioManager.Instance.PlayAudioAtLocation(transform.position, "Pickup");
                        GameManager.Instance.StartDisplayCashCoroutine();

                        GameManager.Instance.IntelCollected++;
                    }
                    //player interacts with an item tagged "shop"
                    else if (hit.collider.CompareTag("Shop"))
                    {

                        GameManager.Instance.buttonFuncScript.OpenShopMenu();
                        currentInteractPrompt.SetActive(false);
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

                        //save that hostage's script to open the hostage door if the hold is completed
                        currentHostage = hit.collider.GetComponent<Hostage>();


                        //reset the progress bar when they press E on the hostage again
                        hostageProgressBarImage.fillAmount = 0;

                        if (currentHostage.doorToOpenWhenHostageSecured != null)
                        {
                            //save the transform of the attached hostage door for playing audio
                            GameManager.Instance.currentHostageDoorTransform = currentHostage.doorToOpenWhenHostageSecured.gameObject.transform;
                        }

                        //play audio
                        AudioManager.Instance.PlayAudioAtLocation(transform.position, "Hostage");
                    }
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

            if(currentHostage != null)
            {
                //tells the corresponding hostage door to open
                currentHostage.HostageSecured();
            }
            else
            {
                Debug.LogError("No hostage script attached to the hostage that was secured.");
            }

            // Disables virtual camera so player can not look around in the pause menu
            if (GameManager.Instance.virtualCam != null)
                GameManager.Instance.virtualCam.SetActive(false);

            //add code to win the level
            GameManager.Instance.GameWon = true;
            ScreenManager.Instance.ShowScreen("XP_Screen");

            //GameManager.Instance.shopCanvas.SetActive(true);
            GameManager.Instance.minimapCanvas.SetActive(false);
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

    public bool SecuringHostage()
    {
        return securingHostage;
    }

}
