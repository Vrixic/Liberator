using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapScanable : MonoBehaviour
{
    [SerializeField]
    GameObject miniMapLocator;

    private void Start()
    {
        miniMapLocator.SetActive(false);
    }

    public void Show()
    {
        miniMapLocator.SetActive(true);
    }

    public void Disable()
    {
        miniMapLocator.SetActive(false);
    }
}
