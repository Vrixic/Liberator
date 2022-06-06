using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefManager : MonoBehaviour
{
    [HideInInspector]
    public float masterVolume = 100;
    [HideInInspector]
    public float musicVolume = 100;
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
    [HideInInspector]
    public int flashBangCapacity;
    [HideInInspector]
    public int sensorGrenadeCapacity;
    public AsyncOperation SceneOperation { get; set; }

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
            Debug.LogError("Multiple Player Pref Mangers! Destroying the newest one: " + this.name);
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        LoadGame();
        DontDestroyOnLoad(this.gameObject);
    }

    /* 
    * Loads player perferences
    */
    public void LoadGame()
    {

        LoadSettings();
        LoadPlayerUpgrades();

    }


    public void LoadSettings()
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
    }

    public void LoadPlayerUpgrades()
    {
        #region Player Upgrades and Xp preferences

        // Skill Points
        if (PlayerPrefs.HasKey("Skill Points"))
        {

            currentSkillPoints = PlayerPrefs.GetInt("Skill Points", 0);
            Debug.Log("Loading Skill Points, Current: " + currentSkillPoints);
        }
        else
        {
            currentSkillPoints = 0;
            PlayerPrefs.SetInt("Skill Points", currentSkillPoints);
        }

        // Player XP
        if (PlayerPrefs.HasKey("Current XP"))
        {

            currentXP = PlayerPrefs.GetInt("Current XP", 1000);
            Debug.Log("Loading Player XP, Current : " + currentXP);
        }
        else
        {
            currentXP = 0;
            PlayerPrefs.SetInt("Current XP", currentXP);
        }

        // Starting Cash 
        if (PlayerPrefs.HasKey("Upgraded Starting Cash"))
        {

            startingCash = PlayerPrefs.GetInt("Upgraded Starting Cash", 1000);
            Debug.Log("Loading Starting Cash, Current : " + startingCash);
        }
        else
        {
            startingCash = 1000;
            PlayerPrefs.SetInt("Upgraded Starting Cash", startingCash);
        }

        // Flashbang Capacity 
        if (PlayerPrefs.HasKey("Flashbang Capacity"))
        {

            flashBangCapacity = PlayerPrefs.GetInt("Flashbang Capacity", 2);
            Debug.Log("Loading Flashbang Capacity, Current : " + flashBangCapacity);
        }
        else
        {
            flashBangCapacity = 2;
            PlayerPrefs.SetInt("Flashbang Capacity", flashBangCapacity);
        }

        // Sensor Grenade Capacity 
        if (PlayerPrefs.HasKey("Sensor Grenade Capacity"))
        {

            sensorGrenadeCapacity = PlayerPrefs.GetInt("Sensor Grenade Capacity", 2);
            Debug.Log("Loading Sensor Grenade Capacity, Current : " + sensorGrenadeCapacity);
        }
        else
        {
            sensorGrenadeCapacity = 2;
            PlayerPrefs.SetInt("Sensor Grenade Capacity", sensorGrenadeCapacity);
        }

        #endregion
    }


    /* 
    * Saves player perferences
    */
    public void SaveGame()
    {
        // Save Player Settings
        PlayerPrefs.SetFloat("Master Volume", masterVolume);
        PlayerPrefs.SetFloat("Music Volume", musicVolume);
        PlayerPrefs.SetFloat("SFX Volume", sfxVolume);
        PlayerPrefs.SetFloat("Brightness", brightness);
        PlayerPrefs.SetFloat("Player Sensitivity", playerSensitivity);

        // Save Player Upgrades and Xp Info
        PlayerPrefs.SetInt("Skill Points", currentSkillPoints);
        PlayerPrefs.SetInt("Current XP", currentXP);
        PlayerPrefs.SetInt("Upgraded Starting Cash", startingCash);
        PlayerPrefs.SetInt("Flashbang Capacity", flashBangCapacity);
        PlayerPrefs.SetInt("Sensor Grenade Capacity", sensorGrenadeCapacity);

        // Fires an event to all listening clients that player perferences has been updated
        OnOptionsUpdateAction?.Invoke();
        Debug.Log("Game Saved");
    }



    public void ResetAllPlayerPrefs()
    {
        // Resetting Stats to default
        PlayerPrefs.SetFloat("Master Volume", 100);
        PlayerPrefs.SetFloat("Music Volume", 100);
        PlayerPrefs.SetFloat("SFX Volume", 100);
        PlayerPrefs.SetFloat("Player Sensitivity", 100);
        PlayerPrefs.SetFloat("Brightness", 100);


        // Resetting player upgrades and xp back to defaults
        PlayerPrefs.SetInt("Skill Points", 0);
        PlayerPrefs.SetInt("Current XP", 0);
        PlayerPrefs.SetInt("Upgraded Starting Cash", 1000);
        PlayerPrefs.SetInt("Flashbang Capacity", 2);
        PlayerPrefs.SetInt("Sensor Grenade Capacity", 2);

        LoadGame();
    }
}
