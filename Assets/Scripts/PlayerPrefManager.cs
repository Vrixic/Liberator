using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefManager : MonoBehaviour
{
    [HideInInspector]
    public float masterVolume = 100;
    [HideInInspector]
    public float musicVolume = 0f;
    [HideInInspector]
    public float sfxVolume = 0f;
    [HideInInspector]
    public float brightness = 0f;
    [HideInInspector]
    public float playerSensitivity = 100;
    [HideInInspector]
    public int CurrentXP { get { return currentXP; } set { currentXP = value; } }
    private int currentXP = 0;
    [HideInInspector]
    public int currentSkillPoints;
    [HideInInspector]
    public int startingCash;

    public Action OnOptionsUpdateAction;

    private static PlayerPrefManager instance;


    public static PlayerPrefManager Instance
    {
        get
        {
            return instance;
        }

        private set
        {
            instance = value;
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple GameManagers! Destroying the newest one: " + this.name);
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        LoadGame();
    }

    /* 
    * Loads player perferences
    */
    public void LoadGame()
    {
        #region Load Settings
        // Master Volume
        if (PlayerPrefs.HasKey("Master Volume"))
        {
            masterVolume = PlayerPrefs.GetFloat("Master Volume", 100f);
        }
        else
        {
            masterVolume = 100f;
            PlayerPrefs.SetFloat("Master Volume", masterVolume);
        }

        // Music Volume
        if (PlayerPrefs.HasKey("Music Volume"))
        {
            musicVolume = PlayerPrefs.GetFloat("Music Volume", 100f);
        }
        else
        {
            musicVolume = 100f;
            PlayerPrefs.SetFloat("Music Volume", musicVolume);
        }

        // SFX volume
        if (PlayerPrefs.HasKey("SFX Volume"))
        {
            sfxVolume = PlayerPrefs.GetFloat("SFX Volume", 100f);
        }
        else
        {
            sfxVolume = 100f;
            PlayerPrefs.SetFloat("SFX Volume", sfxVolume);
        }

        // Player Sensitivity
        if (PlayerPrefs.HasKey("Player Sensitivity"))
        {
            playerSensitivity = PlayerPrefs.GetFloat("Player Sensitivity", 100f);
        }
        else
        {
            playerSensitivity = 100f;
            PlayerPrefs.SetFloat("Player Sensitivity", playerSensitivity);
        }

        // Brightness
        if (PlayerPrefs.HasKey("Brightness"))
        {
            brightness = PlayerPrefs.GetFloat("Brightness", 100f);
        }
        else
        {
            brightness = 100f;
            PlayerPrefs.SetFloat("Brightness", brightness);
        }
        #endregion

        // Skill Points
        if (PlayerPrefs.HasKey("Skill Points"))
        {
            currentSkillPoints = PlayerPrefs.GetInt("Skill Points", 100);
        }
        else
        {
            currentSkillPoints = 0;
            PlayerPrefs.SetInt("Skill Points", currentSkillPoints);
        }

        // Starting Cash 
        if (PlayerPrefs.HasKey("Upgraded Starting Cash"))
        {
            startingCash = PlayerPrefs.GetInt("Skill Points", 1000);
        }
        else
        {
            startingCash = 0;
            PlayerPrefs.SetInt("Skill Points", startingCash);
        }

    }

    /* 
    * Saves player perferences
    */
    public void SaveGame()
    {
        SaveMasterVolume();
        SaveMusicVolume();
        SaveSFXVolume();
        SaveBrightness();
        SavePlayerSensitivity();

        // Fires an event to all listening clients that player perferences has been updated
        OnOptionsUpdateAction?.Invoke();
    }

    /* 
    * Saves master volume
    */
    private void SaveMasterVolume()
    {
        PlayerPrefs.SetFloat("Master Volume", masterVolume);

    }

    /* 
   * Saves music volume
   */
    private void SaveMusicVolume()
    {
        PlayerPrefs.SetFloat("Music Volume", musicVolume);

    }

    /* 
   * Saves sfx volume
   */
    private void SaveSFXVolume()
    {
        PlayerPrefs.SetFloat("SFX Volume", sfxVolume);

    }
    /* 
   * Saves brightness
   */
    private void SaveBrightness()
    {
        PlayerPrefs.SetFloat("Brightness", brightness);

    }

    /* 
    * Saves player sensitivity
    */
    private void SavePlayerSensitivity()
    {
        PlayerPrefs.SetFloat("Player Sensitivity", playerSensitivity);
    }
}
