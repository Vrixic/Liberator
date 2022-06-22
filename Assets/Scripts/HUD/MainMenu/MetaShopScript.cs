using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MetaShopScript : MonoBehaviour
{
    public TMP_Text skillPointCount;
    public TMP_Text equipmentEffectivenessText;
    private void OnEnable()
    {

        if (PlayerPrefManager.Instance.equipmentEffectivenessUpgradeCount >= 5)
        {
            equipmentEffectivenessText.text = "Max Upgrade Achieved";

        }
        else
        {
            equipmentEffectivenessText.text = "Improve Equipment effectiveness: \n5 Renown";

        }
        EventSystem.current.SetSelectedGameObject(GetComponentInChildren<Button>().gameObject);

        PlayerPrefManager.Instance.LoadPlayerUpgrades();
        UpdateSkillPointCount();
    }



    // Updates Skill Point Text to represent current skill points, Add at end of upgrade methods to properly update menu with new skill point count
    public void UpdateSkillPointCount()
    {
        skillPointCount.text = "Renown: " + PlayerPrefManager.Instance.currentSkillPoints;
    }

    #region MetaShop Upgrades Methods

    #region Player Upgrades
    public void UpgradeStartingCash()
    {
        
        if (PlayerPrefManager.Instance.currentSkillPoints == 0)
        {
            return;
        }

        // Check if player has enough skill points to afford upgrade
        if (PlayerPrefs.GetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints) >= 4)
        {

            // Subtract 2 Skill points and add 1000 to starting cash in Player Pref Manager
            PlayerPrefManager.Instance.currentSkillPoints -= 4;
            PlayerPrefManager.Instance.startingCash += 250;

            // Set player pref to new skill point and capacity values
            PlayerPrefs.SetInt("Upgraded Starting Cash", PlayerPrefManager.Instance.startingCash);
            PlayerPrefs.SetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints);


            // Update Skill point count Ui
            UpdateSkillPointCount();
        }

    }
    public void UpgradeStartingHealth()
    {
        if (PlayerPrefManager.Instance.currentSkillPoints == 0)
        {
            return;
        }

        // Check if player has enough skill points to afford upgrade
        if (PlayerPrefs.GetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints) >= 5)
        {

            PlayerPrefManager.Instance.currentSkillPoints -= 5;
            PlayerPrefManager.Instance.playerStartingHealth += 20;

            // Set player pref to new skill point and capacity values
            PlayerPrefs.SetInt("Player Starting Health", PlayerPrefManager.Instance.playerStartingHealth);
            PlayerPrefs.SetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints);


            // Update Skill point count Ui
            UpdateSkillPointCount();
        }
    }

    public void UpgradeStartingArmor()
    {
        if (PlayerPrefManager.Instance.currentSkillPoints == 0)
        {
            return;
        }

        // Check if player has enough skill points to afford upgrade
        if (PlayerPrefs.GetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints) >= 5)
        {

            PlayerPrefManager.Instance.currentSkillPoints -= 5;
            PlayerPrefManager.Instance.playerStartingArmor += 20;

            // Set player pref to new skill point and capacity values
            PlayerPrefs.SetInt("Player Starting Armor", PlayerPrefManager.Instance.playerStartingArmor);
            PlayerPrefs.SetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints);


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
            return;
        }

        // Check if player has enough skill points to afford upgrade
        if (PlayerPrefs.GetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints) >= 3)
        {

            // Subtract 2 Skill points and add one to Capacity in Player Pref Manager
            PlayerPrefManager.Instance.currentSkillPoints -= 3;
            PlayerPrefManager.Instance.flashBangCapacity += 1;

            // Set player pref to new skill point and capacity values
            PlayerPrefs.SetInt("Flashbang Capacity", PlayerPrefManager.Instance.flashBangCapacity);
            PlayerPrefs.SetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints);


            // Update Skill point count Ui
            UpdateSkillPointCount();
        }

    }

    public void UpgradeSensorGrenadeCapacity()
    {
        if (PlayerPrefManager.Instance.currentSkillPoints == 0)
        {
            return;
        }

        // Check if player has enough skill points to afford upgrade
        if (PlayerPrefs.GetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints) >= 3)
        {

            // Subtract 2 Skill points and add one to Capacity in Player Pref Manager
            PlayerPrefManager.Instance.currentSkillPoints -= 3;
            PlayerPrefManager.Instance.sensorGrenadeCapacity += 1;

            // Set player pref to new skill point and capacity values
            PlayerPrefs.SetInt("Sensor Grenade Capacity", PlayerPrefManager.Instance.sensorGrenadeCapacity);
            PlayerPrefs.SetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints);


            // Update Skill point count Ui
            UpdateSkillPointCount();
        }
    }

    // Upgrades equipment range as well as how long the effects of the equipment lasts
    public void UpgradeEquipmentEffectiveness()
    {

        if (PlayerPrefManager.Instance.equipmentEffectivenessUpgradeCount < 5)
        {

            if (PlayerPrefManager.Instance.currentSkillPoints == 0)
            {
                return;
            }

            // Check if player has enough skill points to afford upgrade
            if (PlayerPrefs.GetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints) >= 5)
            {

                PlayerPrefManager.Instance.currentSkillPoints -= 5;

                PlayerPrefManager.Instance.equipmentEffectiveness += 1;
                PlayerPrefManager.Instance.equipmentRange += 1;
                PlayerPrefManager.Instance.equipmentEffectivenessUpgradeCount++;

                // Set player pref to new skill point and values
                PlayerPrefs.SetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints);
                PlayerPrefs.SetInt("Equipment Effectiveness", PlayerPrefManager.Instance.equipmentEffectiveness);
                PlayerPrefs.SetInt("Equipment Range", PlayerPrefManager.Instance.equipmentRange);
                PlayerPrefs.SetInt("Equipment Effectiveness Upgrade Count", PlayerPrefManager.Instance.equipmentEffectivenessUpgradeCount);

                // Update Skill point count Ui
                UpdateSkillPointCount();
            }

        }

        if (PlayerPrefManager.Instance.equipmentEffectivenessUpgradeCount >= 5)
        {
            equipmentEffectivenessText.text = "Max Upgrade Achieved";

        }

    }

    #endregion

    #endregion

    // METHODs FOR TESTING SKILL POINTS BEING SAVED AND META SHOP UPGRADES
    public void Give100SkillPoints()
    {
        PlayerPrefs.SetInt("Skill Points", 100);
        PlayerPrefManager.Instance.currentSkillPoints = PlayerPrefs.GetInt("Skill Points");

        UpdateSkillPointCount();
    }

    public void ResetPlayerUpgradesToDefault()
    {
        PlayerPrefManager.Instance.ResetPlayerUpgrades();
        UpdateSkillPointCount();
    }

    public void SetStartingCash10000()
    {
        PlayerPrefs.SetInt("Upgraded Starting Cash", 10000);
        PlayerPrefManager.Instance.startingCash = PlayerPrefs.GetInt("Upgraded Starting Cash");

    }

    public void ResetAllPlayerPrefs()
    {
        PlayerPrefManager.Instance.ResetAllPlayerPrefs();
        UpdateSkillPointCount();
    }


}
