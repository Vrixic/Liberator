using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctionality : MonoBehaviour
{


    GameObject pause;
    GameObject virtualCam;
    GameObject reticle;
    public static bool gameIsPaused = false;

    void Start()
    {
        // Get instances of pause menu, reticle and Virtual cam
        pause = GameManager.Instance.pause;
        virtualCam = GameManager.Instance.virtualCam;
        reticle = GameManager.Instance.reticle;

    }


    #region PauseMenu
    public void PauseGame()
    {
        if (gameIsPaused == false)
        {
            if (GameManager.Instance.intelInteractText != null || GameManager.Instance.closeDoorInteractText != null || GameManager.Instance.openDoorInteractText != null || GameManager.Instance.secureHostageText != null)
            {
                GameManager.Instance.intelInteractText.SetActive(false);
                GameManager.Instance.closeDoorInteractText.SetActive(false);
                GameManager.Instance.openDoorInteractText.SetActive(false);
                GameManager.Instance.secureHostageText.SetActive(false);
            }
            // Turn off Reticle
            reticle.SetActive(false);
            // Turns on Pause menu image
            pause.SetActive(true);
            // Unlock cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            // Freezes time
            Time.timeScale = 0f;
            // Disables virtual camera so player can not look around in the pause menu
            if (virtualCam != null)
            {
                virtualCam.SetActive(false);
            }
            gameIsPaused = true;
        }
        else
        {
            Resume();
        }
    }
    public void Resume()
    {
        if (virtualCam == null)
        {
            Debug.Log("Reticle is Null");
        }
        // Turn Reticle back on
        reticle.SetActive(true);

        // Set Cursor state back to locked and turn visibility off
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Resumes time
        Time.timeScale = 1f;
        // Re Enables players ability to look around and disables the Pause menu UI image
        if (reticle == null)
        {
            Debug.Log("Virtual Cam is Null");
        }
        if (virtualCam != null)
        {

            virtualCam.SetActive(true);
        }
        pause.SetActive(false);
        gameIsPaused = false;
    }

    public void Restart()
    {
        #region Old Restart code (Commented Out)
        //// Resume time
        //Time.timeScale = 1f;
        //// Get instance of Pause menu and turn it off
        //pause = GameManager.Instance.pause;
        //pause.SetActive(false);
        //if (virtualCam != null)
        //{
        //    // Get instance of virtual camera
        //    virtualCam = GameManager.Instance.virtualCam;

        //}
        //// find Instance of Reticle
        //reticle = GameManager.Instance.reticle;
        //// Find active scene and reload it
        //Scene scene = SceneManager.GetActiveScene();
        //SceneManager.LoadScene(scene.name); 
        #endregion
        GameManager.Instance.ResetGame();
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Application is Exiting");
    }
    #endregion

    #region ShopMenu
    // Close Shop menu
    public void CloseShop()
    {
        reticle.SetActive(true);

        // Set Cursor state back to locked and turn visibility off
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Resumes time
        Time.timeScale = 1f;
        // Re Enables players ability to look around and disables the Pause menu UI image
        if (virtualCam != null)
        {
            virtualCam.SetActive(true);

        }
        GameManager.Instance.shopCanvas.SetActive(false);
    }

    // Refill Health
    public void BuyHealth()
    {
        if (GameManager.Instance.CurrentCash >= 100)
        {
            if (GameManager.Instance.playerScript.GetCurrentPlayerHealth() != GameManager.Instance.playerScript.GetPlayersMaxHealth())
            {
                GameManager.Instance.CurrentCash -= 100;
                UpdateCashCountShopUi();
                GameManager.Instance.playerScript.IncreasePlayerHealth(GameManager.Instance.playerScript.GetPlayersMaxHealth());
                // Updates Healthbar UI
                GameManager.Instance.healthBarScript.UpdateHealthBar();
            }
        }

    }

    // Refill Shield
    public void BuyShield()
    {

        if (GameManager.Instance.CurrentCash >= 200)
        {
            if (GameManager.Instance.playerScript.GetCurrentPlayerShield() != GameManager.Instance.playerScript.GetPlayersMaxShield())
            {
                GameManager.Instance.CurrentCash -= 200;
                UpdateCashCountShopUi();
                GameManager.Instance.playerScript.IncreasePlayerShield(GameManager.Instance.playerScript.GetPlayersMaxShield());
                // Updates Shieldbar UI
                GameManager.Instance.shieldBarScript.UpdateShieldBar();
            }
        }
    }

    // Refill Ammo
    public void BuyAmmo()
    {

        if (GameManager.Instance.CurrentCash >= 100)
        {
            if (AmmoManager.Instance.GetAmmoAmount(AmmoType.Small) != AmmoManager.Instance.GetAmmoCapacity(AmmoType.Small))
            {

                GameManager.Instance.CurrentCash -= 100;
                UpdateCashCountShopUi();
                // TODO: Insert code to reset ammo count
                AmmoManager.Instance.RefillAmmo(AmmoType.Small);
                // TODO: add code that Updates Ui count
                AmmoManager.Instance.UpdateAmmoGUI(AmmoType.Small, AmmoManager.Instance.GetAmmoAmount(AmmoType.Small));

            }

        }
    }

    // Refill FlashBang
    public void BuyFlashBang()
    {

        if (GameManager.Instance.CurrentCash >= 100)
        {
            if (GameManager.Instance.playerScript.GetCurrentFlashbangsAmount() != GameManager.Instance.playerScript.GetMaxFlashBangs())
            {

                GameManager.Instance.CurrentCash -= 100;
                UpdateCashCountShopUi();
                // TODO: Insert code to reset ammo count
                GameManager.Instance.playerScript.IncreaseFlashbang(GameManager.Instance.playerScript.GetMaxFlashBangs());
                // TODO: add code that Updates Ui count
                GameManager.Instance.playerScript.UpdateFlashbangCount();

            }
        }
    }

    // Refill All Equipment
    public void BuyAllEquipment()
    {

        if (GameManager.Instance.CurrentCash >= 200)
        {
            GameManager.Instance.CurrentCash -= 200;
            UpdateCashCountShopUi();

            // TODO: Insert code to refill all equipment count

            // TODO: add code that Updates Ui count
        }
    }

    // To Do: Implement a sensor grenade buy once sensor grenade is implemented
    public void BuySensorGrenade()
    {

        if (GameManager.Instance.CurrentCash >= 100)
        {
            GameManager.Instance.CurrentCash -= 100;
            UpdateCashCountShopUi();

            // TODO: Insert code to refills Sensor grenade count

            // TODO: add code that Updates Ui count
        }
    }
    public void UpdateCashCountShopUi()
    {
        GameManager.Instance.cashCountText.text = "Cash: " + GameManager.Instance.CurrentCash;
    }
    #endregion



}
