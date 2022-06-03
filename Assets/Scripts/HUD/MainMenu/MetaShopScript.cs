using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MetaShopScript : MonoBehaviour
{
    public TMP_Text skillPointCount;
    int currentSkillPoints = 0;
    int startingCash;
    private void OnEnable()
    {
        if (PlayerPrefs.HasKey("Skill Points"))
        {
            PlayerPrefs.GetInt("Skill Points", currentSkillPoints);
        }
        else
        {
            Debug.Log("Skill Point player pref not found");
        }

        // Starting Cash 
        if (PlayerPrefs.HasKey("Upgraded Starting Cash"))
        {
            startingCash = PlayerPrefs.GetInt("Upgraded Starting Cash", 1000);
        }
        else
        {
            startingCash = 0;
            PlayerPrefs.SetInt("Upgraded Starting Cashs", startingCash);
        }
    }

    public void UpdateSkillPointCount()
    {
        PlayerPrefs.SetInt("Skill Points", currentSkillPoints);
        currentSkillPoints = PlayerPrefs.GetInt("Skill Points", currentSkillPoints);
        skillPointCount.text = "Skill Points: " + currentSkillPoints;

    }

    public void UpgradeStartingCash()
    {

    }
}
