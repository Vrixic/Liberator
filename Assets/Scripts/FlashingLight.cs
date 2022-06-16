using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingLight : MonoBehaviour
{
    [SerializeField] GameObject sphere;

    Material sphereMaterial;
    private new Light light;

    private void Start()
    {
        light = GetComponent<Light>();
        sphereMaterial = sphere.GetComponent<Renderer>().material;

        StartCoroutine(FlashLocator());
    }

    public void Disable()
    {
        StopCoroutine(FlashLocator());
        sphereMaterial.color = Color.black; 
        sphereMaterial.SetColor("_EmissionColor", Color.black);
        light.gameObject.SetActive(false);
    }


    IEnumerator FlashLocator()
    {
        Color color = Color.red;
        while (true)
        {
            for (float a = 1f; a > -0.1f; a -= Time.deltaTime * 0.75f)
            {
                light.intensity = a;
                sphereMaterial.SetColor("_EmissionColor", color * a);
                yield return null;
            }

            for (float a = -0.1f; a < 1f; a += Time.deltaTime * 0.75f)
            {
                light.intensity = a;
                sphereMaterial.SetColor("_EmissionColor", color * a);
                yield return null;
            }
        }
    }
}
