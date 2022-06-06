using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRecoil : MonoBehaviour
{
    //these values will be passed in from BaseWeapon.cs whenever a weapon is fired
    public static float incomingVerticalRecoil = 0f;
    public static float incomingHorizontalRecoil = 0f;

    [SerializeField] private float verticalRecoilRestoration = 23f;
    [SerializeField] private float horizontalRecoilRestoration = 10f;

    private float recoilTimer = 0f; //keeps track of when recoil is being applied as rotation to PlayerHeadPos
    private float recoilSmoothDampTime = 0.05f; //time it takes to apply recoil rotation to PlayerHeadPos(recoilTimer is reset to this)

    private Vector2 currentVector; //the rotation vector that has already been applied to PlayerHeadPos
    private Vector2 recoilVector; //the new vector(whether adding recoil or stabilizing) that will alter PlayerHeadPos rotation
    private Vector2 velocityCurrent; //just something you have to pass into smooth damp

    private float timePassed = 0f; //used to keep track of Time.deltaTime for frame-rate independence

    // Update is called once per frame
    void Update()
    {
        //store Time.deltaTime
        timePassed = Time.deltaTime;

        //constantly decrement this timer that will be reset when any recoil is generated
        recoilTimer -= timePassed;

        //if there is no rotation applied upwards to the PlayerHeadPos, and there is no incoming recoil, break out of update
        if(Mathf.Abs(transform.localRotation.x) <= Mathf.Epsilon && Mathf.Abs(incomingVerticalRecoil) <= Mathf.Epsilon && Mathf.Abs(incomingHorizontalRecoil) <= Mathf.Epsilon) 
        {
            return; 
        }

        //if there is some incoming recoil
        if(Mathf.Abs(incomingHorizontalRecoil) > Mathf.Epsilon || Mathf.Abs(incomingVerticalRecoil) > Mathf.Epsilon)
        {
            GenerateRecoil();
        }
        //if there's no incoming recoil and previous recoil is not still being applied as rotation to PlayerHeadPos(recoilTimer == 0)
        else if(recoilTimer + 0.2f <= Mathf.Epsilon) //added 0.05f because it would vertically stablize in between assault rifle shots
        {
            Stabilize();
        }

        //smoothly translate to match the target recoil vector
        currentVector = Vector2.SmoothDamp(currentVector, recoilVector, ref velocityCurrent, recoilSmoothDampTime);

        transform.localRotation = Quaternion.Euler(currentVector.x, currentVector.y, 0f);
    }

    //runs if there is some incoming recoil, will rotate PlayerHeadPos locally, utilizing smoothDamp to apply recoil over a short duration
    private void GenerateRecoil()
    {
        //reset timer since recoil is going to be generated
        recoilTimer = recoilSmoothDampTime;

        //build recoil vector
        recoilVector.x += -incomingVerticalRecoil;
        recoilVector.y += incomingHorizontalRecoil;

        //empty out incoming recoil variables since we have stored the values
        incomingVerticalRecoil = 0f;
        incomingHorizontalRecoil = 0f;

    }

    //runs if there is no recoil being applied to PlayerHeadPos, resets the rotation incrementally towards it's inital local Rotation (0,0,0)
    private void Stabilize()
    {
        //set vertical recoil stabilization
        if (-currentVector.x > verticalRecoilRestoration * timePassed)
        {
            //restore a chunk of vertical recoil
            recoilVector.x += verticalRecoilRestoration * timePassed;
        }
        else
        {
            //restore the rest of the vertical recoil
            recoilVector.x = 0f;
        }

        //set horizontal recoil stabilization
        if (Mathf.Abs(currentVector.y) > horizontalRecoilRestoration * timePassed)
        {
            //horizontal recoil to the right needs to be restored
            if (currentVector.y > Mathf.Epsilon)
            {
                recoilVector.y -= horizontalRecoilRestoration * timePassed;
            }
            //horizontal recoil to the left needs to be restored
            else
            {
                recoilVector.y += horizontalRecoilRestoration * timePassed;
            }
        }
        else
        {
            //restore the rest of the horizontal recoil
            recoilVector.y = 0f;

            GameManager.Instance.playerScript.SetCurrentRecoilIndex(0);
        }
    }
}
