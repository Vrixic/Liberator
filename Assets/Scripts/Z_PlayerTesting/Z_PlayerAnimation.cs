using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Z_PlayerAnimation : MonoBehaviour
{
    Animator playerAnimator;
    string groundTag = "Untagged";

    float footStepWalkAudioPlayDelay = 0.2f;
    float m_LastStepSoundTime = 0f;

    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
    }

    public void PlayAnimation(Vector2 input)
    {
        if (!GameManager.Instance.playerIsGrounded) { 
            playerAnimator.SetBool("isJumping", true);
            return; 
        }
        else if (GameManager.Instance.playerIsGrounded && playerAnimator.GetBool("isJumping") == true){ 
            playerAnimator.SetBool("isJumping", false);
            AudioManager.Instance.PlayAudioAtLocation(GameManager.Instance.playerScript.transform.position, GetGroundsTag(), 3f, true);
        }

        if (GameManager.Instance.playerIsGrounded)
        {
            playerAnimator.SetFloat("VelocityX", input.x);
            playerAnimator.SetFloat("VelocityZ", input.y);
            if (GameManager.Instance.playerMoveScript.IsShifting())
            {
                playerAnimator.speed = 0.5f;
            }
            else playerAnimator.speed = 1;
        }
    }

    void PlayFootStepSound()
    {
        if (!GameManager.Instance.playerIsGrounded) return;

        AudioManager.Instance.PlayAudioAtLocation(GameManager.Instance.playerScript.transform.position, GetGroundsTag());
        //if (GameManager.Instance.playerMoveScript.currentActiveSpeed2D > 0.2f && (Time.time - m_LastStepSoundTime) > footStepWalkAudioPlayDelay)
        //{
        //    m_LastStepSoundTime = Time.time;
            
        //}
    }

    string GetGroundsTag()
    {
        RaycastHit tag;
        Ray checkGroundTag = new Ray(GameManager.Instance.playerScript.transform.position, Vector3.down);
        if (Physics.Raycast(checkGroundTag, out tag, 2f))
        {
            groundTag = tag.collider.tag;
            Debug.Log(tag.collider.tag);
        }
        return groundTag;
    }
}
