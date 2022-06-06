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
        AudioManager.Instance.PlayAudioAtLocation(GameManager.Instance.playerScript.transform.position, "ButtonPress");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlayAudioAtLocation(GameManager.Instance.playerScript.transform.position, "ButtonHover");
    }

    #region PauseMenu
    public void PauseGame()
    {

        if (GameManager.Instance.isPauseMenuOpen == false && !GameManager.Instance.settingsMenu.activeInHierarchy && GameManager.Instance.isShopMenuOpen == false)
        {
            if (GameManager.Instance.intelInteractText != null || GameManager.Instance.closeDoorInteractText != null || GameManager.Instance.openDoorInteractText != null || GameManager.Instance.secureHostageText != null)
            {
                GameManager.Instance.intelInteractText.SetActive(false);
                GameManager.Instance.closeDoorInteractText.SetActive(false);
                GameManager.Instance.openDoorInteractText.SetActive(false);
                GameManager.Instance.secureHostageText.SetActive(false);
            }
            //pause game music
            AudioManager.Instance.PauseMusic();

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
            GameManager.Instance.isPauseMenuOpen = true;
        }
        else if (GameManager.Instance.isShopMenuOpen)
        {
            CloseShop();
        }
        else
        {
            Resume();
        }
    }

    public void Resume()
    {
        if (!GameManager.Instance.settingsMenu.activeInHierarchy)
        {

            //RESUME GAME MUSIC
            AudioManager.Instance.ResumeMusic();

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
        // This code Reloads the scene
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
        UpdateCashCountShopUi();
        GameManager.Instance.isShopMenuOpen = true;

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

    public void LoadNextLevel()
    {
        // Check if player has pressed next level once before, If they haven't it sets the player's position to the beginning of the next level
        Debug.Log(GameManager.Instance.playerTransform.position);
        if (!nextLevelPressedOnce)
        {
            // ** COMMENTED CODE TO TELEPORT PLAYER TO LOCATION FOR NEXT LEVEL ** 
            //nextLevelPosition = new Vector3(-143, 2.461f, 97.97f);
            //GameManager.Instance.playerMoveScript.SetPlayerPosition(nextLevelPosition);

            nextLevelPressedOnce = true;
            SceneManager.LoadScene(2);
            CloseShop();
            Debug.Log(GameManager.Instance.playerTransform.position);
        }
        else
        {
            // Returns player to main menu if they have already played the last level
            ReturnToMainMenu();
        }
    }

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
            if (AmmoManager.Instance.GetAmmoAmount(AmmoType.Shells) == shotgunAmmoCapacity || AmmoManager.Instance.GetAmmoAmount(AmmoType.Small) == SmallAmmoCapacity)
            {
                Debug.Log("ammo at capacity.");
                return;
            }

            if (AmmoManager.Instance.GetAmmoAmount(AmmoType.Shells) != AmmoManager.Instance.GetAmmoCapacity(AmmoType.Shells) || AmmoManager.Instance.GetAmmoAmount(AmmoType.Small) != AmmoManager.Instance.GetAmmoCapacity(AmmoType.Small))
            {
                // Subtract Cost of items and update Cash Count
                GameManager.Instance.CurrentCash -= 100;
                UpdateCashCountShopUi();
                // Checks if player has shotgun equipped then refills Players Ammo to max depending on ammo type
                if (GameManager.Instance.playerScript.GetCurrentEquippedGun().GetWeaponID() == WeaponID.Shotgun)
                {
                    AmmoManager.Instance.RefillAmmo(AmmoType.Shells);

                }
                else
                {
                    AmmoManager.Instance.RefillAmmo(AmmoType.Small);
                }

                // Update Ammo Ui
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
        if (GameManager.Instance.CurrentCash >= 500)
        {
            if (!GameManager.Instance.isCurrentWeaponUpgraded)
            {
                Debug.Log(GameManager.Instance.playerScript.GetCurrentEquippedGun() + " Damage before Upgrade: " + GameManager.Instance.playerScript.GetCurrentEquippedGun().GetDamage());
                GameManager.Instance.CurrentCash -= 500;
                UpdateCashCountShopUi();
                // Gets current weapon equipped and increases the damage of the gun by 25
                GameManager.Instance.playerScript.GetCurrentEquippedGun().SetDamage(GameManager.Instance.playerScript.GetCurrentEquippedGun().GetDamage() + 25);
                // Sets a bool that stores whether a weapon has been upgraded or not then turns on the text to represent the current weapon has been upgraded
                GameManager.Instance.isCurrentWeaponUpgraded = true;
                GameManager.Instance.weaponMaxUpgradeText.enabled = true;
                GameManager.Instance.weaponUpgradeText.enabled = false;
                Debug.Log(GameManager.Instance.playerScript.GetCurrentEquippedGun() + " Damage after Upgrade: " + GameManager.Instance.playerScript.GetCurrentEquippedGun().GetDamage());

            }
        }
    }


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
        Debug.Log("Weapon before Purchase " + GameManager.Instance.playerScript.GetCurrentEquippedGun());
        // Check if player can afford Purchase
        if (GameManager.Instance.CurrentCash >= 500)
        {
            // Check if player already has weapon equipped
            if (GameManager.Instance.playerScript.GetCurrentEquippedGun().GetWeaponID() != WeaponID.Pistol2)
            {
                GameManager.Instance.CurrentCash -= 500;
                UpdateCashCountShopUi();

                // Switches player's current weapon to a pistol
                GameManager.Instance.playerScript.Equip(WeaponID.Pistol2);
                // Reset Weapon is upgraded Boolean 
                GameManager.Instance.isCurrentWeaponUpgraded = false;

                Debug.Log("Weapon after Purchase " + GameManager.Instance.playerScript.GetCurrentEquippedGun());

            }
        }

    }

    public void BuyAssaultRifle()
    {
        Debug.Log("Weapon before Purchase " + GameManager.Instance.playerScript.GetCurrentEquippedGun());
        // Check if player can afford Purchase
        if (GameManager.Instance.CurrentCash >= 500)
        {
            if (GameManager.Instance.playerScript.GetCurrentEquippedGun().GetWeaponID() != WeaponID.Automatic_Rifle3)
            {
                GameManager.Instance.CurrentCash -= 500;
                UpdateCashCountShopUi();

                // Switches player's current weapon to an assualt rife
                GameManager.Instance.playerScript.Equip(WeaponID.Automatic_Rifle3);
                // Force player to equip newly purchase weapon
                // Reset Weapon is upgraded Boolean 
                GameManager.Instance.isCurrentWeaponUpgraded = false;

                Debug.Log("Weapon after Purchase " + GameManager.Instance.playerScript.GetCurrentEquippedGun());

            }
        }

    }

    public void BuyScar()
    {
        Debug.Log("Weapon before Purchase " + GameManager.Instance.playerScript.GetCurrentEquippedGun());
        // Check if player can afford Purchase
        if (GameManager.Instance.CurrentCash >= 500)
        {
            if (GameManager.Instance.playerScript.GetCurrentEquippedGun().GetWeaponID() != WeaponID.Automatic_Rifle)
            {
                GameManager.Instance.CurrentCash -= 500;
                UpdateCashCountShopUi();

                // Switches player's current weapon to a Scar
                GameManager.Instance.playerScript.Equip(WeaponID.Automatic_Rifle);
                // Force player to equip newly purchase weapon
                GameManager.Instance.playerScript.ForceEquipWeapon(1);
                // Reset Weapon is upgraded Boolean 
                GameManager.Instance.isCurrentWeaponUpgraded = false;

                Debug.Log("Weapon after Purchase " + GameManager.Instance.playerScript.GetCurrentEquippedGun());

            }
        }

    }
    public void BuyShotgun()
    {
        Debug.Log("Weapon before Purchase " + GameManager.Instance.playerScript.GetCurrentEquippedGun());
        // Check if player can afford Purchase
        if (GameManager.Instance.CurrentCash >= 500)
        {
            if (GameManager.Instance.playerScript.GetCurrentEquippedGun().GetWeaponID() != WeaponID.Shotgun)
            {
                GameManager.Instance.CurrentCash -= 500;
                UpdateCashCountShopUi();

                // Switches player's current weapon to a Shotgun
                GameManager.Instance.playerScript.Equip(WeaponID.Shotgun);
                // Force player to equip newly purchase weapon
                GameManager.Instance.playerScript.ForceEquipWeapon(1);
                // Reset Weapon is upgraded Boolean 
                GameManager.Instance.isCurrentWeaponUpgraded = false;

                Debug.Log("Weapon after Purchase " + GameManager.Instance.playerScript.GetCurrentEquippedGun());

            }
        }

    }

    #endregion

    #endregion

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    //temporary for alpha sprint
    public void LoadShowcaseLevel()
    {
        SceneManager.LoadScene(3);
    }

}
