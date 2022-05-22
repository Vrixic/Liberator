using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PlayerLook : MonoBehaviour
{
    public static float xRotation = 0f;

    [SerializeField] private float xSensitivity = 30f;
    [SerializeField] private float ySensitivity = 30f;

    private Vector2 currentInputVector;
    private Vector2 recoilInputVector;
    private Vector2 smoothInputWithRecoil;

    public static float pendingXRecoil = 0;
    public static float pendingYRecoil = 0;

    private static float totalYRecoil = 0;
    private static float totalXRecoil = 0;
    private static float yRecoilRecovery = 600f;
    private static float xRecoilRecovery = 200f;
    private static float currentXRecovery = 0f;
    private static float currentYRecovery = 0f;

    public void ProcessLook(Vector2 input)
    {
        //recieve mouse input from the user
        currentInputVector.x = input.x * xSensitivity * Time.deltaTime;
        currentInputVector.y = input.y * ySensitivity * Time.deltaTime;

        currentXRecovery = xRecoilRecovery * Time.deltaTime;
        currentYRecovery = yRecoilRecovery * Time.deltaTime;

        //add on any pending horizontal recoil
        recoilInputVector.x = currentInputVector.x + pendingXRecoil;

        //track the total applied recoil
        totalXRecoil += pendingXRecoil;

        if (totalXRecoil == 0)
        {
            //if there is no horizontal recoil then skip
        }
        //horizontal recoil to the right that needs to be stabilized
        else if (totalXRecoil >= currentXRecovery)
        {
            recoilInputVector.x -= currentXRecovery;
            totalXRecoil -= currentXRecovery;
        }
        //horizontal recoil to the left that needs to be stabilized
        else if (totalXRecoil <= -currentXRecovery)
        {
            recoilInputVector.x += currentXRecovery;
            totalXRecoil += currentXRecovery;
        }
        //little bit of right horizontal recoil to restore
        else if (currentXRecovery > totalXRecoil && totalXRecoil > 0)
        {
            recoilInputVector.x -= totalXRecoil;
            totalXRecoil = 0;
        }
        //little bit of left horizontal recoil to restore
        else if (currentXRecovery > -totalXRecoil && totalXRecoil < 0)
        {
            recoilInputVector.x += totalXRecoil;
            totalXRecoil = 0;
        }
        
        //add on any pending vertical recoil
        recoilInputVector.y = currentInputVector.y + pendingYRecoil;

        totalYRecoil += pendingYRecoil;

        if(totalYRecoil <= 0.1f)
        {
            //if no recoil, do nothing
            GameManager.Instance.playerScript.SetCurrentRecoilIndex(0);
        }
        //significant amount of vertical recoil to restore overtime
        else if (totalYRecoil >= currentYRecovery)
        {
            recoilInputVector.y -= currentYRecovery;
            totalYRecoil -= currentYRecovery;
        }
        //small amount of vertical recoil to restore
        else if (currentYRecovery > totalYRecoil)
        {
            recoilInputVector.y -= totalYRecoil;
            totalYRecoil = 0;
        }

        //smoothly rotate to match the target recoil vector
        currentInputVector = Vector2.SmoothDamp(currentInputVector, recoilInputVector, ref smoothInputWithRecoil, 0.1f);

        //reset pending recoil since it has already been added to the recoilInputVector
        pendingXRecoil = 0;
        pendingYRecoil = 0;

        //rotate the player to look left and right
        transform.Rotate(currentInputVector.x * Vector3.up);

        //calculate camera rotation for looking up and down
        xRotation -= currentInputVector.y;

        //keep the player from breaking their neck trying to look too far up or down
        xRotation = Mathf.Clamp(xRotation, -60f, 80f);

        GameManager.Instance.mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //cam.transform.Rotate(Vector3.left, yRotation * Time.deltaTime);

    }

    public void LowerXSensitivity()
    {
        xSensitivity -= .25f;
    }

    public void RaiseXSensitivity()
    {
        xSensitivity += .25f;
    }
}
