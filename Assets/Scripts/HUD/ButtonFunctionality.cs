using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ButtonFunctionality : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    GameObject pause;
    GameObject virtualCam;
    GameObject reticle;
    int shotgunAmmoCapacity;
    int SmallAmmoCapacity;
    bool nextLevelPressedOnce = false;
    void Start()
    {
        // Get instances of pause menu, reticle and Virtual cam
        pause = GameManager.Instance.pause;
        virtualCam = GameManager.Instance.virtualCam;
        reticle = GameManager.Instance.reticle;
        shotgunAmmoCapacity = AmmoManager.Instance.GetAmmoCapacity(AmmoType.Shells);
        SmallAmmoCapacity = AmmoManager.Instance.GetAmmoCapacity(AmmoType.Small);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.Play2dAudioOnce("ButtonPress");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.Play2dAudioOnce("ButtonHover");
    }

    #region PauseMenu
    public void PauseGame()
    {

        if (GameManager.Instance.canOpenPauseMenu == true && GameManager.Instance.isPauseMenuOpen == false && !GameManager.Instance.settingsMenu.activeInHierarchy && GameManager.Instance.isShopMenuOpen == false)
        {

            if (GameManager.Instance.playerInteractScript.currentInteractPrompt != null)
            {
                GameManager.Instance.playerInteractScript.currentInteractPrompt.SetActive(false);
            }

            //pause game music
            AudioManager.Instance.PauseMusic();

            // Turn off Reticle
            reticle.SetActive(false);
            // Turns on Pause menu image
            pause.SetActive(true);
            EventSystem.current.SetSelectedGameObject(GetComponentInChildren<Button>().gameObject);

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
            GameManager.Instance.isPauseMenuOpen = true;
        }
        else if (GameManager.Instance.isShopMenuOpen)
        {
            CloseShop();
        }
        else if (!GameManager.Instance.isXPScreenActive)
        {
            Resume();
        }
    }

    public void Resume()
    {
        if (!GameManager.Instance.settingsMenu.activeInHierarchy)
        {

            //RESUME GAME MUSIC
            if (GameManager.Instance.playerScript.GetCurrentPlayerHealth() <= 30)
            {
                AudioManager.Instance.ResumeMusic();
            }

            // Turn Reticle back on
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
            pause.SetActive(false);
            GameManager.Instance.isPauseMenuOpen = false;
        }
    }

    public void Restart()
    {
        #region Old Restart code (Commented Out)

        // Resume time
        Time.timeScale = 1f;
        // Get instance of Pause menu and turn it off
        pause = GameManager.Instance.pause;
        pause.SetActive(false);
        if (virtualCam != null)
        {
            // Get instance of virtual camera
            virtualCam = GameManager.Instance.virtualCam;

        }
        // find Instance of Reticle
        reticle = GameManager.Instance.reticle;
        // Find active scene and reload it
        PlayerPrefManager.Instance.SceneOperation = SceneManager.LoadSceneAsync(1);
        PlayerPrefManager.Instance.SceneOperation.allowSceneActivation = false;
        ScreenManager.Instance.ShowScreen("Transition_Screen");

        #endregion

    }

    public void Quit()
    {
        Application.Quit();
    }
    #endregion

    #region ShopMenu

    // Open Shop Menu
    public void OpenShopMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;

        // Disables virtual camera so player can not look around in game
        if (GameManager.Instance.virtualCam != null)
            GameManager.Instance.virtualCam.SetActive(false);
        GameManager.Instance.minimapCanvas.SetActive(false);
        GameManager.Instance.shopCanvas.SetActive(true);
        if (GameManager.Instance.openShopInteractText.activeInHierarchy)
        {
            GameManager.Instance.openShopInteractText.SetActive(false);
        }
        SetUpgradeWeaponIcon();

        //EventSystem.current.SetSelectedGameObject(GetComponentInChildren<Button>().gameObject);

        UpdateCashCountShopUi();
        GameManager.Instance.isShopMenuOpen = true;


        if (GameManager.Instance.playerInteractScript.currentInteractPrompt != null)
        {
            GameManager.Instance.playerInteractScript.currentInteractPrompt.SetActive(false);
        }
    }

    // Close Shop menu
    public void CloseShop()
    {

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
        GameManager.Instance.reticle.SetActive(true);
        GameManager.Instance.shopCanvas.SetActive(false);
        GameManager.Instance.buyWeaponsCanvas.SetActive(false);
        GameManager.Instance.minimapCanvas.SetActive(true);
        GameManager.Instance.isShopMenuOpen = false;


    }
    private void SetUpgradeWeaponIcon()
    {
        WeaponID weaponID = GameManager.Instance.playerScript.GetCurrentEquippedGun().GetWeaponID();
        if (weaponID != WeaponID.Knife)
        {
            GameManager.Instance.weaponUpgradeButton.GetComponent<Image>().sprite = GameManager.Instance.gunIcon.sprite;

        }

        if (weaponID == WeaponID.Pistol || weaponID == WeaponID.Revolver)
        {
            GameManager.Instance.weaponUpgradeButton.gameObject.transform.localScale = new Vector2(0.4f, 3);
        }
        else
        {
            GameManager.Instance.weaponUpgradeButton.gameObject.transform.localScale = new Vector2(0.7191864f, 2.388015f);
        }

    }

    public void SetCurrentSelectedButton()
    {
        EventSystem.current.SetSelectedGameObject(GetComponentInChildren<Button>().gameObject);
    }

    #region Buy Items Methods
    // Refill Health
    public void BuyHealth()
    {
        if (GameManager.Instance.CurrentCash >= 100)
        {
            if (GameManager.Instance.playerScript.GetCurrentPlayerHealth() != GameManager.Instance.playerScript.GetPlayersMaxHealth())
            {
                // Subtract Cost of items and update Cash Count
                GameManager.Instance.CurrentCash -= 100;
                UpdateCashCountShopUi();
                // Refill players Health to max
                GameManager.Instance.playerScript.IncreasePlayerHealth(GameManager.Instance.playerScript.GetPlayersMaxHealth());
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
                // Subtract Cost of items and update Cash Count
                GameManager.Instance.CurrentCash -= 200;
                UpdateCashCountShopUi();
                // Refill Players Shield to max
                GameManager.Instance.playerScript.IncreasePlayerShield(GameManager.Instance.playerScript.GetPlayersMaxShield());
            }
        }
    }

    // Refill Ammo
    public void BuyAmmo()
    {

        if (GameManager.Instance.CurrentCash >= 100)
        {
            // Checks if player is at max ammo for ammo type small and ammo type shells
            if (AmmoManager.Instance.GetAmmoAmount(AmmoType.Shells) == shotgunAmmoCapacity && AmmoManager.Instance.GetAmmoAmount(AmmoType.Small) == SmallAmmoCapacity)
            {
                return;
            }


            // Subtract Cost of items and update Cash Count
            GameManager.Instance.CurrentCash -= 100;
            UpdateCashCountShopUi();

            // Refills ammo
            AmmoManager.Instance.RefillAmmo(AmmoType.Shells);
            AmmoManager.Instance.RefillAmmo(AmmoType.Small);

            // Update Ammo Ui
            //AmmoManager.Instance.UpdateAmmoGUI(AmmoType.Shells, AmmoManager.Instance.GetAmmoAmount(AmmoType.Shells));
            //AmmoManager.Instance.UpdateAmmoGUI(AmmoType.Small, AmmoManager.Instance.GetAmmoAmount(AmmoType.Small));
        }
    }

    // Refill FlashBang
    public void BuyFlashBang()
    {

        if (GameManager.Instance.CurrentCash >= 100)
        {
            if (GameManager.Instance.playerScript.GetCurrentFlashbangsAmount() != GameManager.Instance.playerScript.GetMaxFlashBangs())
            {
                GameManager.Instance.playerScript.ActivateFlashbangMesh();
                // Subtract Cost of items and update Cash Count
                GameManager.Instance.CurrentCash -= 100;
                UpdateCashCountShopUi();
                // Refill players flashbang Count to max
                GameManager.Instance.playerScript.IncreaseFlashbangAmount(GameManager.Instance.playerScript.GetMaxFlashBangs());
                // Update Flashbang UI
                GameManager.Instance.playerScript.UpdateFlashbangCount();

            }
        }
    }

    // Refill All Equipment
    public void BuyAllEquipment()
    {

        if (GameManager.Instance.CurrentCash >= 200)
        {
            if (GameManager.Instance.playerScript.GetCurrentFlashbangsAmount() != GameManager.Instance.playerScript.GetMaxFlashBangs())
            {
                if (GameManager.Instance.playerScript.GetCurrentSensorGrenadeCount() != GameManager.Instance.playerScript.GetMaxSensorGrenadeCount())
                {
                    GameManager.Instance.playerScript.ActivateFlashbangMesh();
                    GameManager.Instance.playerScript.ActivateSensorMesh();
                    // Subtract Cost of items and update Cash Count
                    GameManager.Instance.CurrentCash -= 200;
                    UpdateCashCountShopUi();
                    // Refill Flashbangs and Sensor Grenades to max
                    GameManager.Instance.playerScript.IncreaseFlashbangAmount(GameManager.Instance.playerScript.GetMaxFlashBangs());
                    GameManager.Instance.playerScript.IncreaseSensorGrenadeAmount(GameManager.Instance.playerScript.GetMaxSensorGrenadeCount());

                    // Update Ui
                    GameManager.Instance.playerScript.UpdateFlashbangCount();
                    GameManager.Instance.playerScript.UpdateSensorGrenadeUi();
                }
            }
        }
    }

    public void BuySensorGrenade()
    {

        if (GameManager.Instance.CurrentCash >= 100)
        {
            if (GameManager.Instance.playerScript.GetCurrentSensorGrenadeCount() != GameManager.Instance.playerScript.GetMaxSensorGrenadeCount())
            {
                GameManager.Instance.playerScript.ActivateSensorMesh();
                // Subtract Cost of items and update Cash Count
                GameManager.Instance.CurrentCash -= 100;
                UpdateCashCountShopUi();
                // Refill Sensor Grenades to max
                GameManager.Instance.playerScript.IncreaseSensorGrenadeAmount(GameManager.Instance.playerScript.GetMaxSensorGrenadeCount());
                // Update Sensor Grenade UI
                GameManager.Instance.playerScript.UpdateSensorGrenadeUi();
            }

        }
    }

    public void UpgradeCurrentWeapon()
    {
        if (GameManager.Instance.CurrentCash >= 750)
        {
            if (!GameManager.Instance.isCurrentWeaponUpgraded)
            {
                // Debug.Log(GameManager.Instance.playerScript.GetCurrentEquippedGun() + " Damage before Upgrade: " + GameManager.Instance.playerScript.GetCurrentEquippedGun().GetDamage());
                GameManager.Instance.CurrentCash -= 750;
                UpdateCashCountShopUi();
                // Gets current weapon equipped and increases the damage of the gun by 25
                GameManager.Instance.playerScript.GetCurrentEquippedGun().SetDamage(GameManager.Instance.playerScript.GetCurrentEquippedGun().GetDamage() + 25);
                // Sets a bool that stores whether a weapon has been upgraded or not then turns on the text to represent the current weapon has been upgraded
                GameManager.Instance.isCurrentWeaponUpgraded = true;
                GameManager.Instance.weaponMaxUpgradeText.enabled = true;
                GameManager.Instance.weaponUpgradeText.enabled = false;
                // Debug.Log(GameManager.Instance.playerScript.GetCurrentEquippedGun() + " Damage after Upgrade: " + GameManager.Instance.playerScript.GetCurrentEquippedGun().GetDamage());

            }
        }
    }

    #endregion

    public void UpdateCashCountShopUi()
    {
        GameManager.Instance.itemTabCashCountText.text = "Cash: " + GameManager.Instance.CurrentCash;
        GameManager.Instance.buyWeaponTabCashCountText.text = "Cash: " + GameManager.Instance.CurrentCash;

    }

    public void ShowItemsTab()
    {
        if (!GameManager.Instance.shopCanvas.activeInHierarchy)
        {
            GameManager.Instance.buyWeaponsCanvas.SetActive(false);
            GameManager.Instance.shopCanvas.SetActive(true);
            SetUpgradeWeaponIcon();
            // Dynamically change Weapon Upgrade Text depending on whether the weapon has been upgraded or not
            if (GameManager.Instance.isCurrentWeaponUpgraded == true)
            {
                GameManager.Instance.weaponMaxUpgradeText.enabled = true;
                GameManager.Instance.weaponUpgradeText.enabled = false;
            }
            else
            {
                GameManager.Instance.weaponMaxUpgradeText.enabled = false;
                GameManager.Instance.weaponUpgradeText.enabled = true;
            }

            UpdateCashCountShopUi();
        }

    }
    public void ShowBuyGunsTab()
    {
        if (!GameManager.Instance.buyWeaponsCanvas.activeInHierarchy)
        {
            GameManager.Instance.shopCanvas.SetActive(false);
            GameManager.Instance.buyWeaponsCanvas.SetActive(true);
            UpdateCashCountShopUi();

        }
    }

    #region Buy Weapons Methods

    public void BuyPistol()
    {
        // Check if player can afford Purchase
        if (GameManager.Instance.CurrentCash >= 500)
        {
            // Check if player already has weapon equipped
            if (GameManager.Instance.playerScript.GetCurrentEquippedGun().GetWeaponID() != WeaponID.Pistol)
            {
                GameManager.Instance.CurrentCash -= 500;
                UpdateCashCountShopUi();

                // Switches player's current weapon to a pistol
                GameManager.Instance.playerScript.Equip(WeaponID.Pistol);
                // Reset Weapon is upgraded Boolean 
                GameManager.Instance.isCurrentWeaponUpgraded = false;


            }
        }

    }

    public void BuyRevolver()
    {
        // Check if player can afford Purchase
        if (GameManager.Instance.CurrentCash >= 750)
        {
            // Check if player already has weapon equipped
            if (GameManager.Instance.playerScript.GetCurrentEquippedGun().GetWeaponID() != WeaponID.Revolver)
            {
                GameManager.Instance.CurrentCash -= 750;
                UpdateCashCountShopUi();

                // Switches player's current weapon to a revolver
                GameManager.Instance.playerScript.Equip(WeaponID.Revolver);
                // Reset Weapon is upgraded Boolean 
                GameManager.Instance.isCurrentWeaponUpgraded = false;

            }
        }

    }

    public void BuyAssaultRifle()
    {
        // Check if player can afford Purchase
        if (GameManager.Instance.CurrentCash >= 1000)
        {
            if (GameManager.Instance.playerScript.GetCurrentEquippedGun().GetWeaponID() != WeaponID.Automatic_Rifle3)
            {
                GameManager.Instance.CurrentCash -= 1000;
                UpdateCashCountShopUi();

                // Switches player's current weapon to an assualt rife
                GameManager.Instance.playerScript.Equip(WeaponID.Automatic_Rifle3);

                // Reset Weapon is upgraded Boolean 
                GameManager.Instance.isCurrentWeaponUpgraded = false;


            }
        }

    }

    public void BuyScar()
    {
        // Check if player can afford Purchase
        if (GameManager.Instance.CurrentCash >= 1000)
        {
            if (GameManager.Instance.playerScript.GetCurrentEquippedGun().GetWeaponID() != WeaponID.Automatic_Rifle)
            {
                GameManager.Instance.CurrentCash -= 1000;
                UpdateCashCountShopUi();

                // Switches player's current weapon to a Scar
                GameManager.Instance.playerScript.Equip(WeaponID.Automatic_Rifle);
                // Force player to equip newly purchase weapon
                GameManager.Instance.playerScript.ForceEquipWeapon(1);
                // Reset Weapon is upgraded Boolean 
                GameManager.Instance.isCurrentWeaponUpgraded = false;


            }
        }

    }
    public void BuyShotgun()
    {
        // Check if player can afford Purchase
        if (GameManager.Instance.CurrentCash >= 1000)
        {
            if (GameManager.Instance.playerScript.GetCurrentEquippedGun().GetWeaponID() != WeaponID.Shotgun)
            {
                GameManager.Instance.CurrentCash -= 1000;
                UpdateCashCountShopUi();

                // Switches player's current weapon to a Shotgun
                GameManager.Instance.playerScript.Equip(WeaponID.Shotgun);
                // Force player to equip newly purchase weapon
                GameManager.Instance.playerScript.ForceEquipWeapon(1);
                // Reset Weapon is upgraded Boolean 
                GameManager.Instance.isCurrentWeaponUpgraded = false;


            }
        }

    }

    #endregion

    #endregion

    public void ReturnToMainMenu()
    {
        PlayerPrefManager.Instance.SceneOperation = SceneManager.LoadSceneAsync(0);
        PlayerPrefManager.Instance.SceneOperation.allowSceneActivation = false;
        ScreenManager.Instance.ShowScreen("Transition_Screen");
        //SceneManager.LoadScene(0);
    }

    //temporary for alpha sprint
    public void LoadShowcaseLevel()
    {
        SceneManager.LoadScene(3);
    }

}
