using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PlayerLook : MonoBehaviour
{
    public static float xRotation = 0f;

    [SerializeField] private float xSensitivity = .5f;
    [SerializeField] private float ySensitivity = .5f;

    private Vector2 currentInputVector;

    public void ProcessLook(Vector2 input)
    {
        //recieve mouse input from the user
        currentInputVector.x = input.x * xSensitivity * Time.deltaTime;
        currentInputVector.y = input.y * ySensitivity * Time.deltaTime;

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
        xSensitivity -= .75f;
        ySensitivity -= .75f;
    }

    public void RaiseXSensitivity()
    {
        xSensitivity += .75f;
        ySensitivity += .75f;
    }

    public void OnSensitivityUpdate()
    {
        Debug.Log("X Sens before: " + xSensitivity);
        Debug.Log("y-Sens before: " + ySensitivity);
        xSensitivity = PlayerPrefManager.Instance.playerSensitivity;
        Debug.Log("x-Sens: " + xSensitivity);
        ySensitivity = PlayerPrefManager.Instance.playerSensitivity;
        Debug.Log("y-Sens: " + ySensitivity);

    }
}