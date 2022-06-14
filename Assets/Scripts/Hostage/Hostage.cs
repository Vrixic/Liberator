using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hostage : MonoBehaviour
{
    [SerializeField] public HostageDoorController doorToOpenWhenHostageSecured;
    [SerializeField] private GameObject levelToLoad = null;
    [SerializeField] private GameObject levelToUnload = null;

    public static int hostagesSecured = 0;

    public void HostageSecured()
    {
        hostagesSecured++;

        if(levelToLoad != null)
            levelToLoad.SetActive(true);
        
        if (levelToUnload != null)
            levelToUnload.SetActive(false);

        if(doorToOpenWhenHostageSecured != null)
        {
            doorToOpenWhenHostageSecured.OpenHostageDoor();
        }
        else
        {
            Debug.LogError("No hostage door script attached to this hostage.");
        }

        gameObject.SetActive(false);
    }
}
