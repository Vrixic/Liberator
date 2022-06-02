using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SettingsMenu : MonoBehaviour
{
    public Slider brightnessSlider;
    public Slider volumeSlider;
    public Slider sensitivitySlider;
    public TMP_InputField brightnessInput;
    public TMP_InputField volumeInput;
    public TMP_InputField sensitivityInput;

    float masterVolume = 0f;
    float playerSensitivity = 0f;
    float brightness = 0f;
    private void OnEnable()
    {
        if (PlayerPrefs.HasKey("Master Volume"))
        {
            masterVolume = PlayerPrefs.GetFloat("Master Volume", 1f);
        }
        else
        {
            masterVolume = 100f;
            PlayerPrefs.SetFloat("Master Volume", masterVolume);
        }

        if (PlayerPrefs.HasKey("Player Sensitivity"))
        {
            playerSensitivity = PlayerPrefs.GetFloat("Player Sensitivity", 1f);
        }
        else
        {
            playerSensitivity = 100f;
            PlayerPrefs.SetFloat("Player Sensitivity", playerSensitivity);
        }

        if (PlayerPrefs.HasKey("Brightness"))
        {
            brightness = PlayerPrefs.GetFloat("Brightness", 1f);
        }
        else
        {
            brightness = 100f;
            PlayerPrefs.SetFloat("Brightness", brightness);
        }

        InitSliders();
    }

    // Initialize Sliders and text boxes to current player prefs
    public void InitSliders()
    {
        brightnessInput.text = brightness.ToString();
        brightnessSlider.value = brightness;

        sensitivityInput.text = playerSensitivity.ToString();
        sensitivitySlider.value = playerSensitivity;

        volumeInput.text = masterVolume.ToString();
        volumeSlider.value = masterVolume;
    }

   
    #region Update Sliders and Text Fields Methods
    public void UpdateBrightnessInputValue()
    {
        // If player inputs "00" or "000" it defaults to only show one 0
        if (brightnessInput.text == "00" || brightnessInput.text == "000")
        {
            brightnessInput.text = "0";
        }
        // Updates Player preference to value inputted into text box
        PlayerPrefs.SetFloat("Brightness", int.Parse(brightnessInput.text));
        // Updates Slider to represent value inputted into text box
        brightnessSlider.value = int.Parse(brightnessInput.text);
    }

    public void UpdateVolumeInputValue()
    {
        // If player inputs "00" or "000" it defaults to only show one 0
        if (volumeInput.text == "00" || volumeInput.text == "000")
        {
            volumeInput.text = "0";
        }
        // Updates Player preference to value inputted into text box

        PlayerPrefs.SetFloat("Master Volume", int.Parse(volumeInput.text));
        
        // Updates Slider to represent value inputted into text box
        volumeSlider.value = int.Parse(volumeInput.text);
    }
    public void UpdateSensitivityInputValue()
    {
        // If player inputs "00" or "000" it defaults to only show one 0
        if (sensitivityInput.text == "00" || sensitivityInput.text == "000")
        {
            sensitivityInput.text = "0";
        }

        // Updates Player preference to value inputted into text box
        PlayerPrefs.SetFloat("Player Sensitivity", int.Parse(sensitivityInput.text));

        // Updates Slider to represent value inputted into text box
        sensitivitySlider.value = int.Parse(sensitivityInput.text);
    }

    public void UpdateBrightnessSliderValue()
    {
        // Updates Player preference to value set by slider

        PlayerPrefs.SetFloat("Brightness", brightnessSlider.value);

        // Updates text box to represent number input by slider
        brightnessInput.text = brightnessSlider.value.ToString();
    }

    public void UpdateVolumeSliderValue()
    {
        // Updates Player preference to value set by slider

        PlayerPrefs.SetFloat("Master Volume", volumeSlider.value);

        // Updates text box to represent number input by slider
        volumeInput.text = volumeSlider.value.ToString();

    }

    public void UpdateSensitivitySliderValue()
    {
        // Updates Player preference to value set by slider

        PlayerPrefs.SetFloat("Player Sensitivity", sensitivitySlider.value);

        // Updates text box to represent number input by slider
        sensitivityInput.text = sensitivitySlider.value.ToString();

    }

    #endregion
}
