using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeathScreen : BaseScreen
{
    [SerializeField] RawImage deathImage;

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

        ScreenManager.Instance.ShowScreen("XP_Screen");

        StartCoroutine(FadeOutDeathImage());
    }

    IEnumerator FadeOutDeathImage()
    {
        Color c = deathImage.color;
        for (float alpha = 0; alpha < 0.24f; alpha = deathImage.color.a)
        {
            alpha += Time.unscaledDeltaTime * 0.1f;

            c.a = alpha;
            deathImage.color = c;

            yield return null;
        }
    }
}
