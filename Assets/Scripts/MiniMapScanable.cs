using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapScanable : MonoBehaviour
{
    [SerializeField]
    GameObject miniMapLocator;
    Material locatorMaterial;

    bool bIsAlreadyShowing = false;

    [SerializeField] float locationExposedTimer = 5f;

    private void Start()
    {
        miniMapLocator.SetActive(false);
        locatorMaterial = miniMapLocator.GetComponent<Renderer>().material;
    }

    public void Show()
    {
        if (bIsAlreadyShowing) return;

        miniMapLocator.SetActive(true);
        StartCoroutine(FlashLocator(locationExposedTimer));

        bIsAlreadyShowing = true;
    }

    public void Disable()
    {
        miniMapLocator.SetActive(false);
        bIsAlreadyShowing = false;
    }

    IEnumerator FlashLocator(float time)
    {
        Color color = Color.red;
        while (time >= 0)
        {
            for (float a = 1f; a > -0.1f; a -= Time.deltaTime * 0.75f)
            {
                locatorMaterial.SetColor("_EmissionColor", color * a);

                yield return null;
            }

            time -= 1f;

            for (float a = -0.1f; a < 1f; a += Time.deltaTime * 0.75f)
            {
                locatorMaterial.SetColor("_EmissionColor", color * a);

                yield return null;
            }

            time -= 1f;

            yield return null;
        }
        Disable();
    }
}
