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

        PlayerPrefManager.Instance.LoadPlayerUpgrades();
        UpdateSkillPointCount();
    }

  

    // Updates Skill Point Text to represent current skill points, Add at end of upgrade methods to properly update menu with new skill point count
    public void UpdateSkillPointCount()
    {
        skillPointCount.text = "Skill Points: " + PlayerPrefManager.Instance.currentSkillPoints;
    }

    #region MetaShop Upgrades Methods

    #region Player Upgrades
    public void UpgradeStartingCash()
    {
        if (PlayerPrefManager.Instance.currentSkillPoints == 0)
        {
            Debug.Log("player has zero Skill points");
            return;
        }

        // Check if player has enough skill points to afford upgrade
        if (PlayerPrefs.GetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints) >= 4)
        {
            Debug.Log("Upgrading Starting Cash by $250");
            Debug.Log("Skill points before: " + PlayerPrefManager.Instance.currentSkillPoints + " starting cash before: " + PlayerPrefManager.Instance.startingCash);

            // Subtract 2 Skill points and add 1000 to starting cash in Player Pref Manager
            PlayerPrefManager.Instance.currentSkillPoints -= 4;
            PlayerPrefManager.Instance.startingCash += 1000;

            // Set player pref to new skill point and capacity values
            PlayerPrefs.SetInt("Upgraded Starting Cash", PlayerPrefManager.Instance.startingCash);
            PlayerPrefs.SetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints);

            Debug.Log("Skill points after: " + PlayerPrefManager.Instance.currentSkillPoints + " starting cash after: " + PlayerPrefManager.Instance.startingCash);

            // Update Skill point count Ui
            UpdateSkillPointCount();
        }

    }
    public void UpgradeStartingHealth()
    {
        if (PlayerPrefManager.Instance.currentSkillPoints == 0)
        {
            Debug.Log("player has zero Skill points");
            return;
        }

        // Check if player has enough skill points to afford upgrade
        if (PlayerPrefs.GetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints) >= 4)
        {
            Debug.Log("Upgrading Player Starting Health by 100");
            Debug.Log("Skill points before: " + PlayerPrefManager.Instance.currentSkillPoints + " starting health before: " + PlayerPrefManager.Instance.playerStartingHealth);

            PlayerPrefManager.Instance.currentSkillPoints -= 4;
            PlayerPrefManager.Instance.playerStartingHealth += 100;

            // Set player pref to new skill point and capacity values
            PlayerPrefs.SetInt("Player Starting Health", PlayerPrefManager.Instance.playerStartingHealth);
            PlayerPrefs.SetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints);

            Debug.Log("Skill points after: " + PlayerPrefManager.Instance.currentSkillPoints + " starting health after: " + PlayerPrefManager.Instance.playerStartingHealth);

            // Update Skill point count Ui
            UpdateSkillPointCount();
        }
    }

    public void UpgradeStartingArmor()
    {
        if (PlayerPrefManager.Instance.currentSkillPoints == 0)
        {
            Debug.Log("player has zero Skill points");
            return;
        }

        // Check if player has enough skill points to afford upgrade
        if (PlayerPrefs.GetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints) >= 4)
        {
            Debug.Log("Upgrading Player Starting Armor by 100");
            Debug.Log("Skill points before: " + PlayerPrefManager.Instance.currentSkillPoints + " starting armor before: " + PlayerPrefManager.Instance.playerStartingArmor);

            PlayerPrefManager.Instance.currentSkillPoints -= 4;
            PlayerPrefManager.Instance.playerStartingArmor += 100;

            // Set player pref to new skill point and capacity values
            PlayerPrefs.SetInt("Player Starting Armor", PlayerPrefManager.Instance.playerStartingArmor);
            PlayerPrefs.SetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints);

            Debug.Log("Skill points after: " + PlayerPrefManager.Instance.currentSkillPoints + " starting armor after: " + PlayerPrefManager.Instance.playerStartingArmor);

            // Update Skill point count Ui
            UpdateSkillPointCount();
        }
    } 

    #endregion

    #region Equipment Related Upgrades
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
            PlayerPrefs.SetInt("Flashbang Capacity", PlayerPrefManager.Instance.flashBangCapacity);
            PlayerPrefs.SetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints);

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
            PlayerPrefs.SetInt("Sensor Grenade Capacity", PlayerPrefManager.Instance.sensorGrenadeCapacity);
            PlayerPrefs.SetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints);

            Debug.Log("Skill points after: " + PlayerPrefManager.Instance.currentSkillPoints + " Sensor Grenade Capacity after: " + PlayerPrefManager.Instance.sensorGrenadeCapacity);

            // Update Skill point count Ui
            UpdateSkillPointCount();
        }
    }

    // Upgrades equipment range as well as how long the effects of the equipment lasts
    public void UpgradeEquipmentEffectiveness()
    {
        if (PlayerPrefManager.Instance.currentSkillPoints == 0)
        {
            Debug.Log("player has zero Skill points");
            return;
        }

        // Check if player has enough skill points to afford upgrade
        if (PlayerPrefs.GetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints) >= 4)
        {
            Debug.Log("Upgrading Equipment Effectiveness by ");
            Debug.Log("Skill points before: " + PlayerPrefManager.Instance.currentSkillPoints + " equip effectiveness before: " + PlayerPrefManager.Instance.equipmentEffectiveness);
            Debug.Log("Upgrading Equipment Range by ");
            Debug.Log(" equip range before: " + PlayerPrefManager.Instance.equipmentRange);

            PlayerPrefManager.Instance.currentSkillPoints -= 4;

            PlayerPrefManager.Instance.equipmentEffectiveness += 1;
            PlayerPrefManager.Instance.equipmentRange += 1;

            // Set player pref to new skill point and capacity values
            PlayerPrefs.SetInt("Equipment Effectiveness", PlayerPrefManager.Instance.equipmentEffectiveness);
            PlayerPrefs.SetInt("Equipment Range", PlayerPrefManager.Instance.equipmentRange);
            PlayerPrefs.SetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints);

            Debug.Log("Skill points after: " + PlayerPrefManager.Instance.currentSkillPoints + " equip effectiveness after: " + PlayerPrefManager.Instance.equipmentEffectiveness);
            Debug.Log(" equip range before: " + PlayerPrefManager.Instance.equipmentRange);
            // Update Skill point count Ui
            UpdateSkillPointCount();
        }

    }

    #endregion

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
        PlayerPrefs.SetInt("Player Starting Health", 100);
        PlayerPrefs.SetInt("Player Starting Armor", 100);
        PlayerPrefs.SetInt("Equipment Effectiveness", 5);
        PlayerPrefs.SetInt("Equipment Range", 10);

        // Reset counts for amount of times player has purchased upgrades
        PlayerPrefs.SetInt("Starting Cash Upgrade Count", 0);
        PlayerPrefs.SetInt("Flashbang Capacity Upgrade Count", 0);
        PlayerPrefs.SetInt("Sensor Grenade Capacity Upgrade Count", 0);
        PlayerPrefs.SetInt("Equipment Effectiveness Upgrade Count", 0);
        PlayerPrefs.SetInt("Starting Health Upgrade Count", 0);
        PlayerPrefs.SetInt("Starting Armor Upgrade Count", 0);

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
