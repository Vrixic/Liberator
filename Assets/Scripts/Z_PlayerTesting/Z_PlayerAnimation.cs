using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Z_PlayerAnimation : MonoBehaviour
{
    Animator playerAnimator;
    string groundTag = "Untagged";
    bool bJustPaused = false;
    bool bCanPlaySound = true;

    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Time.timeScale < 0.01f && !bJustPaused)
        {
            bJustPaused = true;
            bCanPlaySound = false;
        }
        else if (bJustPaused)
        {

            bJustPaused = false;
            Invoke("TogglePlaySound", 0.25f);
        }

    }

    public void TogglePlaySound()
    {
        bCanPlaySound = true;
    }

    public void PlayAnimation(Vector2 input)
    {
        if (!bCanPlaySound) return;
        if (!GameManager.Instance.playerIsGrounded) { 
            playerAnimator.SetBool("isJumping", true);
            return; 
        }
        else if (GameManager.Instance.playerIsGrounded && playerAnimator.GetBool("isJumping") == true){ 
            playerAnimator.SetBool("isJumping", false);
            AudioManager.Instance.PlayAudioAtLocation(GameManager.Instance.playerScript.transform.position, GetGroundsTag(), 4f, true);

        }

        if (GameManager.Instance.playerIsGrounded)
        {
            playerAnimator.SetFloat("VelocityX", input.x);
            playerAnimator.SetFloat("VelocityZ", input.y);
            if (GameManager.Instance.playerMoveScript.IsShifting() || GameManager.Instance.playerMoveScript.IsCrouching())
            {
                playerAnimator.speed = 0.5f;
            }
            else playerAnimator.speed = 1;
        }
    }

    void PlayFootStepSound()
    {
        if (!bCanPlaySound) return;
        if (!GameManager.Instance.playerIsGrounded || GameManager.Instance.playerMoveScript.IsShifting() || GameManager.Instance.playerMoveScript.IsCrouching()) return;
        
        AudioManager.Instance.PlayAudioAtLocation(GameManager.Instance.playerScript.transform.position, GetGroundsTag());
        if (Random.Range(0 ,3) == 0)
        {
            AudioManager.Instance.PlayAudioAtLocation(GameManager.Instance.playerScript.transform.position, "PlayerMovingSoundEffects");
        }
    }

    string GetGroundsTag()
    {
        RaycastHit tag;
        Ray checkGroundTag = new Ray(GameManager.Instance.playerScript.transform.position, Vector3.down);
        if (Physics.Raycast(checkGroundTag, out tag, 2f))
        {
            groundTag = tag.collider.tag;
        }
        return groundTag;
    }
}
