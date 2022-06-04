using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MetaShopScript : MonoBehaviour
{
    public TMP_Text skillPointCount;
    private void OnEnable()
    {
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

        // *********************** TEMP GIVE 100 SKILL POINTS, DISABLE AFTER TESTING IS FINISHED **********************************************************************
        Give100SkillPoints();
        // UNCOMMENT BELOW TO RETURN PLAYER UPGRADES TO DEFAULT VALUES
        //ResetPlayerUpgradesToDefault();
        UpdateSkillPointCount();
    }

    public void UpdateSkillPointCount()
    {
        //PlayerPrefs.SetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints);
        //PlayerPrefManager.Instance.currentSkillPoints = PlayerPrefs.GetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints);

        skillPointCount.text = "Skill Points: " + PlayerPrefManager.Instance.currentSkillPoints;

    }

    public void UpgradeStartingCash()
    {
        if (PlayerPrefs.GetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints) >= 2)
        {
            Debug.Log("Upgrading Starting Cash by $250");
            Debug.Log("Skill points before: " + PlayerPrefManager.Instance.currentSkillPoints + " starting cash before: " + PlayerPrefManager.Instance.startingCash);
            PlayerPrefManager.Instance.currentSkillPoints -= 2;
            PlayerPrefManager.Instance.startingCash += 250;
            PlayerPrefs.SetInt("Upgraded Starting Cash", PlayerPrefManager.Instance.startingCash);
            PlayerPrefs.SetInt("Skill Points", PlayerPrefManager.Instance.currentSkillPoints);
            Debug.Log("Skill points after: " + PlayerPrefManager.Instance.currentSkillPoints + " starting cash after: " + PlayerPrefManager.Instance.startingCash);
            UpdateSkillPointCount();
        }

    }

    public void UpgradeFlashbangCapacity()
    {

    }

    public void UpgradeSensorGrenadeCapacity()
    {

    }

    // METHODs FOR TESTING SKILL POINTS BEING SAVED AND META SHOP UPGRADES
    private void Give100SkillPoints()
    {
        PlayerPrefs.SetInt("Skill Points", 100);
    }

    private void ResetPlayerUpgradesToDefault()
    {
        PlayerPrefs.SetInt("Skill Points", 0);
        PlayerPrefs.SetInt("Upgraded Starting Cash", 1000);


    }
}
