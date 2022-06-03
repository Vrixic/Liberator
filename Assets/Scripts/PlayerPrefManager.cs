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
    public float sfxVolume = 100;
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
            Debug.Log("Loading Master Volume Setting, Current: " + masterVolume);
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
            Debug.Log("Loading music Volume Setting, Current: " + musicVolume);

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
            Debug.Log("Loading SFX Volume Setting, Current : " + sfxVolume);
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
            Debug.Log("Loading Sensitivity, current: " + playerSensitivity);

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
            Debug.Log("Loading Brightness Setting, Current: " + brightness);
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
            Debug.Log("Loading Skill Points, Current: " + currentSkillPoints);
        }
        else
        {
            currentSkillPoints = 0;
            PlayerPrefs.SetInt("Skill Points", currentSkillPoints);
        }

        // Starting Cash 
        if (PlayerPrefs.HasKey("Upgraded Starting Cash"))
        {

            startingCash = PlayerPrefs.GetInt("Upgraded Starting Cash", 1000);
            Debug.Log("Loading Starting Cash, Current : " + startingCash);
        }
        else
        {
            startingCash = 0;
            PlayerPrefs.SetInt("Upgraded Starting Cash, Current; ", startingCash);
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
        SaveStartingCash();
        SaveSkillPoints();

        // Fires an event to all listening clients that player perferences has been updated
        OnOptionsUpdateAction?.Invoke();
        Debug.Log("Game Saved");
    }

    #region Save Settings
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
    #endregion

    #region Save Player Info
    /* 
    * Saves Skill Points
    */
    private void SaveSkillPoints()
    {
        PlayerPrefs.SetInt("Skill Points", currentSkillPoints);

    }

    private void SaveStartingCash()
    {
        PlayerPrefs.SetInt("Upgraded Starting Cash", startingCash);

    }
    #endregion
}
