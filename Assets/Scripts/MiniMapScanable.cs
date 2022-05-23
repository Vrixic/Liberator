using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapScanable : MonoBehaviour
{
    [SerializeField]
    GameObject miniMapLocator;
    Material locatorMaterial;

    [SerializeField] float locationExposedTimer = 5f;

    private void Start()
    {
        miniMapLocator.SetActive(false);
        locatorMaterial = miniMapLocator.GetComponent<Renderer>().material;
    }

    public void Show()
    {
        miniMapLocator.SetActive(true);
        StartCoroutine(FlashLocator(locationExposedTimer));
    }

    public void Disable()
    {
        miniMapLocator.SetActive(false);
    }

    IEnumerator FlashLocator(float time)
    {
        bool fadingOut = true;
        Color color = locatorMaterial.color;
        color.a = 50;
        while (time >= 0)
        {
            if (fadingOut)
                color.a -= 4f;
            else
                color.a += 5f;

            if (color.a < 0)
                fadingOut = false;
            if (color.a > 50)
                fadingOut = true;

            locatorMaterial.color = color;
            time -= 0.05f;

            yield return new WaitForSeconds(0.05f);
        }
        Disable();
    }
}
