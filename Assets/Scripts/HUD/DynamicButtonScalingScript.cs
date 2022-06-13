using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DynamicButtonScalingScript : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    Vector3 originalScale;
    float xScaler = .19f;
    float yScaler = .03f;
    float zScaler = .19f;
    public bool isBiggerButtton = false;
    public bool isMenuButton = false;
    private void Start()
    {
        originalScale = gameObject.transform.localScale;
        if (isBiggerButtton)
        {
            xScaler = .15f;
            yScaler = .01f;
            zScaler = .15f;
        }

        if (isMenuButton)
        {
            xScaler = .1f;
            yScaler = .01f;
            zScaler = .1f;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.transform.localScale = new Vector3(originalScale.x + xScaler, originalScale.x + yScaler, originalScale.z + zScaler);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.localScale = originalScale;
    }
}
