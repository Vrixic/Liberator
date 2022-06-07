using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MetaShopScript : MonoBehaviour
{
    public TMP_Text skillPointCount;
    private void OnEnable()
    {


        EventSystem.current.SetSelectedGameObject(GetComponentInChildren<Button>().gameObject);


        // Skill points
        if (PlayerPrefs.HasKey("Skill Points"))
        {
            PlayerPrefs.GetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints);
        }
        else
        {
            Debug.Log("Skill Point player pref not found");
        }

        // Starting Cash 
        if (PlayerPrefs.HasKey("Upgraded Starting Cash"))
        {
            PlayerPrefManager.Instance.startingCash = PlayerPrefs.GetInt("Upgraded Starting Cash", 1000);
        }
        else
        {
            Debug.Log("Upgraded Starting Cash player pref not found");
        }


        // Flashbang Capacity 
        if (PlayerPrefs.HasKey("Flashbang Capacity"))
        {

            PlayerPrefManager.Instance.flashBangCapacity = PlayerPrefs.GetInt("Flashbang Capacity", 1000);
            Debug.Log("Loading Flashbang Capacity, Current : " + PlayerPrefManager.Instance.flashBangCapacity);
        }
        else
        {
            Debug.Log("Flashbang Capacity player pref not found");

        }

        // Sensor Grenade Capacity 
        if (PlayerPrefs.HasKey("Sensor Grenade Capacity"))
        {

            PlayerPrefManager.Instance.sensorGrenadeCapacity = PlayerPrefs.GetInt("Sensor Grenade Capacity", 2);
            Debug.Log("Loading Sensor Grenade Capacity, Current : " + PlayerPrefManager.Instance.sensorGrenadeCapacity);
        }
        else
        {
            Debug.Log("Sensor Grenade Capacity player pref not found");
        }



        UpdateSkillPointCount();
    }

    // Updates Skill Point Text to represent current skill points, Add at end of upgrade methods to properly update menu with new skill point count
    public void UpdateSkillPointCount()
    {
        skillPointCount.text = "Skill Points: " + PlayerPrefManager.Instance.currentSkillPoints;
    }

    #region MetaShop Upgrades Methods

    public void UpgradeStartingCash()
    {
        if (PlayerPrefManager.Instance.currentSkillPoints == 0)
        {
            Debug.Log("player has zero Skill points");
            return;
        }

        // Check if player has enough skill points to afford upgrade
        if (PlayerPrefs.GetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints) >= 2)
        {
            Debug.Log("Upgrading Starting Cash by $250");
            Debug.Log("Skill points before: " + PlayerPrefManager.Instance.currentSkillPoints + " starting cash before: " + PlayerPrefManager.Instance.startingCash);

            // Subtract 2 Skill points and add 250 to starting cash in Player Pref Manager
            PlayerPrefManager.Instance.currentSkillPoints -= 2;
            PlayerPrefManager.Instance.startingCash += 250;

            // Set player pref to new skill point and capacity values
            PlayerPrefs.SetInt("Upgraded Starting Cash", PlayerPrefManager.Instance.startingCash);
            PlayerPrefs.SetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints);

            Debug.Log("Skill points after: " + PlayerPrefManager.Instance.currentSkillPoints + " starting cash after: " + PlayerPrefManager.Instance.startingCash);

            // Update Skill point count Ui
            UpdateSkillPointCount();
        }

    }

    public void UpgradeFlashbangCapacity()
    {
        if (PlayerPrefManager.Instance.currentSkillPoints == 0)
        {
            Debug.Log("player has zero Skill points");
            return;
        }

        // Check if player has enough skill points to afford upgrade
        if (PlayerPrefs.GetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints) >= 2)
        {
            Debug.Log("Upgrading Flashbang Capacity by 1");
            Debug.Log("Skill points before: " + PlayerPrefManager.Instance.currentSkillPoints + " Flashbang Capacity before: " + PlayerPrefManager.Instance.flashBangCapacity);

            // Subtract 2 Skill points and add one to Capacity in Player Pref Manager
            PlayerPrefManager.Instance.currentSkillPoints -= 2;
            PlayerPrefManager.Instance.flashBangCapacity += 1;

            // Set player pref to new skill point and capacity values
            PlayerPrefs.SetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints);
            PlayerPrefs.SetInt("Flashbang Capacity", PlayerPrefManager.Instance.flashBangCapacity);

            Debug.Log("Skill points after: " + PlayerPrefManager.Instance.currentSkillPoints + " Flashbang Capacity after: " + PlayerPrefManager.Instance.flashBangCapacity);

            // Update Skill point count Ui
            UpdateSkillPointCount();
        }

    }

    public void UpgradeSensorGrenadeCapacity()
    {
        if (PlayerPrefManager.Instance.currentSkillPoints == 0)
        {
            Debug.Log("player has zero Skill points");
            return;
        }

        // Check if player has enough skill points to afford upgrade
        if (PlayerPrefs.GetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints) >= 2)
        {
            Debug.Log("Upgrading Sensor Capacity by 1");
            Debug.Log("Skill points before: " + PlayerPrefManager.Instance.currentSkillPoints + " Sensor Grenade Capacity before: " + PlayerPrefManager.Instance.sensorGrenadeCapacity);

            // Subtract 2 Skill points and add one to Capacity in Player Pref Manager
            PlayerPrefManager.Instance.currentSkillPoints -= 2;
            PlayerPrefManager.Instance.sensorGrenadeCapacity += 1;

            // Set player pref to new skill point and capacity values
            PlayerPrefs.SetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints);
            PlayerPrefs.SetInt("Sensor Grenade Capacity", PlayerPrefManager.Instance.sensorGrenadeCapacity);

            Debug.Log("Skill points after: " + PlayerPrefManager.Instance.currentSkillPoints + " Sensor Grenade Capacity after: " + PlayerPrefManager.Instance.sensorGrenadeCapacity);

            // Update Skill point count Ui
            UpdateSkillPointCount();
        }
    }
    #endregion

    // METHODs FOR TESTING SKILL POINTS BEING SAVED AND META SHOP UPGRADES
    public void Give100SkillPoints()
    {
        PlayerPrefs.SetInt("Skill Points", 100);
        PlayerPrefManager.Instance.currentSkillPoints = PlayerPrefs.GetInt("Skill Points");
        Debug.Log("Player Skill points: " + PlayerPrefManager.Instance.currentSkillPoints);
        UpdateSkillPointCount();
    }

    public void ResetPlayerUpgradesToDefault()
    {
        PlayerPrefs.SetInt("Skill Points", 0);
        PlayerPrefs.SetInt("Upgraded Starting Cash", 1000);
        PlayerPrefs.SetInt("Flashbang Capacity", 2);
        PlayerPrefs.SetInt("Sensor Grenade Capacity", 2);
        PlayerPrefManager.Instance.LoadPlayerUpgrades();
        UpdateSkillPointCount();

    }

    public void SetStartingCash10000()
    {
        PlayerPrefs.SetInt("Upgraded Starting Cash", 10000);
        PlayerPrefManager.Instance.startingCash = PlayerPrefs.GetInt("Upgraded Starting Cash");
        Debug.Log("Player current starting cash: " + PlayerPrefManager.Instance.startingCash);

    }

    public void ResetAllPlayerPrefs()
    {
        PlayerPrefManager.Instance.ResetAllPlayerPrefs();
        UpdateSkillPointCount();
    }
}
