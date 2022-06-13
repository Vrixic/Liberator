using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hostage : MonoBehaviour
{
    [SerializeField] public HostageDoorController doorToOpenWhenHostageSecured;

    public static int hostagesSecured = 0;

    public void HostageSecured()
    {
        hostagesSecured++;

        if(doorToOpenWhenHostageSecured != null)
        {
            doorToOpenWhenHostageSecured.OpenHostageDoor();
        }
        else
        {
            Debug.LogError("No hostage door script attached to this hostage.");
        }
    }
}
