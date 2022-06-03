using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeathScreen : BaseScreen, IPointerClickHandler
{
    public override void Show()
    {
        base.Show();

        // Disables minimap canvas
        GameManager.Instance.minimapCanvas.SetActive(false);

        GameManager.Instance.healthBar.SetActive(false);
        GameManager.Instance.ShieldBar.SetActive(false);

        GameManager.Instance.ammoCanvas.SetActive(false);

        // unlock mouse 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameManager.Instance.IsUIOverlayVisible = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ScreenManager.Instance.ShowScreen("XP_Screen");
    }
}
