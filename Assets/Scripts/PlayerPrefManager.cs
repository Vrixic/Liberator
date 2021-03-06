using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public int voicePromptState = 1;
    [HideInInspector]
    public int CurrentXP { get { return currentXP; } set { currentXP = value; } }
    private int currentXP = 0;
    [HideInInspector]
    public int currentSkillPoints;
    [HideInInspector]
    public int totalSkillPointsSpent;
    [HideInInspector]
    public int startingCash;
    [HideInInspector]
    public int flashBangCapacity;
    [HideInInspector]
    public int sensorGrenadeCapacity;
    [HideInInspector]
    public int equipmentEffectiveness;
    [HideInInspector]
    public int equipmentRange;
    [HideInInspector]
    public int playerStartingHealth;
    [HideInInspector]
    public int playerStartingArmor;
    [HideInInspector]
    public int equipmentEffectivenessUpgradeCount = 0;
    [HideInInspector]
    public string PlayerName { get; set; } = "null_name";

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
        if (instance != null)
        {
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
            // Debug.Log("Loading Master Volume Setting, Current: " + masterVolume);
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
            // Debug.Log("Loading music Volume Setting, Current: " + musicVolume);

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
            // Debug.Log("Loading SFX Volume Setting, Current : " + sfxVolume);
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
            // Debug.Log("Loading Sensitivity, current: " + playerSensitivity);

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
            // Debug.Log("Loading Brightness Setting, Current: " + brightness);
        }
        else
        {
            brightness = 100f;
            PlayerPrefs.SetFloat("Brightness", brightness);
        }

        // playerPromt On/Off Status
        if (PlayerPrefs.HasKey("Voice Prompts State"))
        {

            voicePromptState = PlayerPrefs.GetInt("Voice Prompts State", 1);
            // Debug.Log("Loading voicePrompt Setting, Current: " + voicePromptState);
        }
        else
        {
            voicePromptState = 1;
            PlayerPrefs.SetFloat("Voice Prompts State", voicePromptState);
        }

        PlayerName = PlayerPrefs.GetString("PlayerName", "null_name");
        #endregion
    }

    public void LoadPlayerUpgrades()
    {
        #region Player Upgrades and Xp preferences

        // Skill Points
        if (PlayerPrefs.HasKey("Skill Points"))
        {

            currentSkillPoints = PlayerPrefs.GetInt("Skill Points", 0);
            // Debug.Log("Loading Skill Points, Current: " + currentSkillPoints);
        }
        else
        {
            currentSkillPoints = 0;
            PlayerPrefs.SetInt("Skill Points", currentSkillPoints);
        }

        // total skill points spent
        if (PlayerPrefs.HasKey("Total Skill Points"))
        {

            totalSkillPointsSpent = PlayerPrefs.GetInt("Total Skill Points", 0);
            // Debug.Log("Loading Skill Points, Current: " + currentSkillPoints);
        }
        else
        {
            totalSkillPointsSpent = 0;
            PlayerPrefs.SetInt("Total Skill Points", totalSkillPointsSpent);
        }

        // Player XP
        if (PlayerPrefs.HasKey("Current XP"))
        {

            currentXP = PlayerPrefs.GetInt("Current XP", 1000);
            // Debug.Log("Loading Player XP, Current : " + currentXP);
        }
        else
        {
            currentXP = 0;
            PlayerPrefs.SetInt("Current XP", currentXP);
        }

        // Starting Cash 
        if (PlayerPrefs.HasKey("Upgraded Starting Cash"))
        {

            startingCash = PlayerPrefs.GetInt("Upgraded Starting Cash", 250);
            // Debug.Log("Loading Starting Cash, Current : " + startingCash);
        }
        else
        {
            startingCash = 250;
            PlayerPrefs.SetInt("Upgraded Starting Cash", startingCash);
        }

        // Flashbang Capacity 
        if (PlayerPrefs.HasKey("Flashbang Capacity"))
        {

            flashBangCapacity = PlayerPrefs.GetInt("Flashbang Capacity", 2);
            // Debug.Log("Loading Flashbang Capacity, Current : " + flashBangCapacity);
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
            // Debug.Log("Loading Sensor Grenade Capacity, Current : " + sensorGrenadeCapacity);
        }
        else
        {
            sensorGrenadeCapacity = 2;
            PlayerPrefs.SetInt("Sensor Grenade Capacity", sensorGrenadeCapacity);
        }

        // Player Starting Health
        if (PlayerPrefs.HasKey("Player Starting Health"))
        {
            playerStartingHealth = PlayerPrefs.GetInt("Player Starting Health");
            // Debug.Log("Loading Player Starting Health, Current : " + playerStartingHealth);
        }
        else
        {
            playerStartingHealth = 100;
            PlayerPrefs.SetInt("Player Starting Health", playerStartingHealth);

        }

        // Player Starting Armor
        if (PlayerPrefs.HasKey("Player Starting Armor"))
        {
            playerStartingArmor = PlayerPrefs.GetInt("Player Starting Armor");
            // Debug.Log("Loading Player Starting Armor, Current : " + playerStartingArmor);
        }
        else
        {
            playerStartingArmor = 100;
            PlayerPrefs.SetInt("Player Starting Armor", playerStartingArmor);
        }

        // Equipment Effectiveness ( how long sensor and flash effect lasts)
        if (PlayerPrefs.HasKey("Equipment Effectiveness"))
        {
            equipmentEffectiveness = PlayerPrefs.GetInt("Equipment Effectiveness");
            // Debug.Log("Loading Equipment Effectiveness, Current : " + equipmentEffectiveness);
        }
        else
        {
            equipmentEffectiveness = 5;
            PlayerPrefs.SetInt("Equipment Effectiveness", equipmentEffectiveness);
        }

        // Equipment range (How big the sensor and flashbangs range is)
        if (PlayerPrefs.HasKey("Equipment Range"))
        {
            equipmentRange = PlayerPrefs.GetInt("Equipment Range");
            // Debug.Log("Loading Equipment Range, Current : " + equipmentRange);
        }
        else
        {
            equipmentRange = 10;
            PlayerPrefs.SetInt("Equipment Range", equipmentRange);
        }

        #endregion

        #region Upgrade Counts


        //Equipment Effectiveness Upgrade Count
        if (PlayerPrefs.HasKey("Equipment Effectiveness Upgrade Count"))
        {
            equipmentEffectivenessUpgradeCount = PlayerPrefs.GetInt("Equipment Effectiveness Upgrade Count");
            // Debug.Log("Loading Equipment Effectiveness Upgrade Count, Current : " + equipmentEffectivenessUpgradeCount);
        }
        else
        {
            equipmentEffectivenessUpgradeCount = 0;
            PlayerPrefs.SetInt("Equipment Effectiveness Upgrade Count", equipmentEffectivenessUpgradeCount);
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
        PlayerPrefs.SetInt("Voice Prompts State", voicePromptState);
        PlayerPrefs.SetInt("Total Skill Points", totalSkillPointsSpent);

        // Save Player Upgrades and Xp Info
        PlayerPrefs.SetInt("Skill Points", currentSkillPoints);
        PlayerPrefs.SetInt("Current XP", currentXP);
        PlayerPrefs.SetInt("Upgraded Starting Cash", startingCash);
        PlayerPrefs.SetInt("Flashbang Capacity", flashBangCapacity);
        PlayerPrefs.SetInt("Sensor Grenade Capacity", sensorGrenadeCapacity);

        // Save amount of times equipment effectiveness upgrades has occured
        PlayerPrefs.SetInt("Equipment Effectiveness Upgrade Count", equipmentEffectivenessUpgradeCount);

        // Fires an event to all listening clients that player perferences has been updated
        OnOptionsUpdateAction?.Invoke();
        // Debug.Log("Game Saved");
    }

    /* Only will be called once -> Saves Players Name */
    public void SavePlayerName()
    {
        PlayerPrefs.SetString("PlayerName", PlayerName);
    }

    public void ResetPlayerUpgrades()
    {
        // Resetting player upgrades and xp back to defaults
        currentSkillPoints = totalSkillPointsSpent + currentSkillPoints;
        totalSkillPointsSpent = 0;
        PlayerPrefs.SetInt("Total Skill Points", totalSkillPointsSpent);
        PlayerPrefs.SetInt("Skill Points", currentSkillPoints);
        PlayerPrefs.SetInt("Current XP", 0);
        PlayerPrefs.SetInt("Upgraded Starting Cash", 250);
        PlayerPrefs.SetInt("Flashbang Capacity", 2);
        PlayerPrefs.SetInt("Sensor Grenade Capacity", 2);
        PlayerPrefs.SetInt("Player Starting Health", 100);
        PlayerPrefs.SetInt("Player Starting Armor", 100);
        PlayerPrefs.SetInt("Equipment Effectiveness", 5);
        PlayerPrefs.SetInt("Equipment Range", 10);

        // Resets counts for amount of times upgrades have been purchased to 0
        PlayerPrefs.SetInt("Equipment Effectiveness Upgrade Count", 0);


        LoadPlayerUpgrades();
    }
    public void ResetAllPlayerPrefs()
    {
        // Resetting settings to default
        PlayerPrefs.SetFloat("Master Volume", 100);
        PlayerPrefs.SetFloat("Music Volume", 100);
        PlayerPrefs.SetFloat("SFX Volume", 100);
        PlayerPrefs.SetFloat("Player Sensitivity", 100);
        PlayerPrefs.SetFloat("Brightness", 100);
        PlayerPrefs.SetInt("Voice Prompts State", 1);
        PlayerPrefs.SetInt("Total Skill Points", 0);

        // Reset player name 
        PlayerPrefs.SetString("PlayerName", "null_name");

        ResetPlayerUpgrades();

        LoadGame();
    }
}
