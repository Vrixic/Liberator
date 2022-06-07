using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Z_PlayerAnimation : MonoBehaviour
{
    Animator playerAnimator;
    float x, z;
    string groundTag = "Untagged";

    float footStepWalkAudioPlayDelay = 0.2f;
    float m_LastStepSoundTime = 0f;
    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!GameManager.Instance.playerMoveScript.ReturnIsGrounded()) { 
            playerAnimator.SetBool("isJumping", true);
            return; 
        }
        else if (GameManager.Instance.playerMoveScript.ReturnIsGrounded() && playerAnimator.GetBool("isJumping") == true){ 
            playerAnimator.SetBool("isJumping", false);
            AudioManager.Instance.PlayAudioAtLocation(GameManager.Instance.playerScript.transform.position, GetGroundsTag(), 3f, true);
        }

        if (GameManager.Instance.playerMoveScript.ReturnIsGrounded())
        {
            x = GameManager.Instance.playerMoveScript.currentInputVector.x;
            z = GameManager.Instance.playerMoveScript.currentInputVector.y;
            playerAnimator.SetFloat("VelocityX", x);
            playerAnimator.SetFloat("VelocityZ", z);
            playerAnimator.speed = 0.2f + (GameManager.Instance.playerMoveScript.currentActiveSpeed2D*1.5f);
        }
    }

    void PlayFootStepSound()
    {
        if (!GameManager.Instance.playerMoveScript.ReturnIsGrounded()) return;

        if (GameManager.Instance.playerMoveScript.currentActiveSpeed2D > 0.2 && (Time.time - m_LastStepSoundTime) > footStepWalkAudioPlayDelay)
        {
            m_LastStepSoundTime = Time.time;
            AudioManager.Instance.PlayAudioAtLocation(GameManager.Instance.playerScript.transform.position, GetGroundsTag());
        }
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
