using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctionality : MonoBehaviour
{


    public GameObject pause;
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
            virtualCam.SetActive(false);
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
        virtualCam.SetActive(true);
        pause.SetActive(false);
        gameIsPaused = false;
    }

    public void Restart()
    {
        // Resume time
        Time.timeScale = 1f;
        // Get instance of Pause menu and turn it off
        pause = GameManager.Instance.pause;
        pause.SetActive(false);
        // Get instance of virtual camera
        virtualCam = GameManager.Instance.virtualCam;
        // find Instance of Reticle
        reticle = GameManager.Instance.reticle;
        // Find active scene and reload it
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Application is Exiting");
    }
    #endregion

    #region ShopMenu

    // Refill Health
    public void BuyHealth()
    {
        if (GameManager.Instance.CurrentCash >= 100)
        {
            GameManager.Instance.CurrentCash -= 100;
        }
    }

    // Refill Shield
    public void BuyShield()
    {

        if (GameManager.Instance.CurrentCash >= 200)
        {
            GameManager.Instance.CurrentCash -= 200;

        }
    }

    // Refill Ammo
    public void BuyAmmo()
    {

        if (GameManager.Instance.CurrentCash >= 100)
        {
            GameManager.Instance.CurrentCash -= 100;
        }
    }

    // Refill FlashBang
    public void BuyFlashBang()
    {

        if (GameManager.Instance.CurrentCash >= 100)
        {
            GameManager.Instance.CurrentCash -= 100;
        }
    }

    // Refill All Equipment
    public void BuyAllEquipment()
    {

        if (GameManager.Instance.CurrentCash >= 200)
        {
            GameManager.Instance.CurrentCash -= 200;
        }
    }

    // To Do: Implement a sensor grenade buy once sensor grenade is implemented
    public void BuySensorGrenade()
    {

        if (GameManager.Instance.CurrentCash >= 100)
        {
            GameManager.Instance.CurrentCash -= 100;
        }
    } 
    #endregion




}
