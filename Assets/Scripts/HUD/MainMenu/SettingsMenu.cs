using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SettingsMenu : MonoBehaviour
{
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider brightnessSlider;
    public Slider sensitivitySlider;

    public TMP_InputField masterVolumeInput;
    public TMP_InputField musicVolumeInput;
    public TMP_InputField sfxVolumeInput;
    public TMP_InputField brightnessInput;
    public TMP_InputField sensitivityInput;

    float masterVolume = 0f;
    float musicVolume = 0f;
    float sfxVolume = 0f;
    float playerSensitivity = 0f;
    float brightness = 0f;
    private void OnEnable()
    {
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

        InitSliders();
    }


    // Initialize Sliders and text boxes to current player prefs
    public void InitSliders()
    {
        masterVolumeInput.text = masterVolume.ToString();
        masterVolumeSlider.value = masterVolume;

        musicVolumeInput.text = musicVolume.ToString();
        musicVolumeSlider.value = musicVolume;

        sfxVolumeInput.text = sfxVolume.ToString();
        sfxVolumeSlider.value = sfxVolume;

        sensitivityInput.text = playerSensitivity.ToString();
        sensitivitySlider.value = playerSensitivity;

        brightnessInput.text = brightness.ToString();
        brightnessSlider.value = brightness;
    }

    #region Update Sliders and Text Fields Methods
    public void UpdateMasterVolumeInputValue()
    {
        // If player inputs "00" or "000" it defaults to only show one 0
        if (masterVolumeInput.text == "00" || masterVolumeInput.text == "000")
        {
            masterVolumeInput.text = "0";
        }
        // Updates Player preference to value inputted into text box

        PlayerPrefs.SetFloat("Master Volume", int.Parse(masterVolumeInput.text));
        
        // Updates Slider to represent value inputted into text box
        masterVolumeSlider.value = int.Parse(masterVolumeInput.text);
    }

    public void UpdateMusicVolumeInputValue()
    {
        // If player inputs "00" or "000" it defaults to only show one 0
        if (musicVolumeInput.text == "00" || musicVolumeInput.text == "000")
        {
            musicVolumeInput.text = "0";
        }
        // Updates Player preference to value inputted into text box

        PlayerPrefs.SetFloat("Music Volume", int.Parse(musicVolumeInput.text));

        // Updates Slider to represent value inputted into text box
        musicVolumeSlider.value = int.Parse(musicVolumeInput.text);
    }

    public void UpdateSFXVolumeInputValue()
    {
        // If player inputs "00" or "000" it defaults to only show one 0
        if (sfxVolumeInput.text == "00" || sfxVolumeInput.text == "000")
        {
            sfxVolumeInput.text = "0";
        }
        // Updates Player preference to value inputted into text box

        PlayerPrefs.SetFloat("SFX Volume", int.Parse(sfxVolumeInput.text));

        // Updates Slider to represent value inputted into text box
        sfxVolumeSlider.value = int.Parse(sfxVolumeInput.text);
    }

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

    public void UpdateMasterVolumeSliderValue()
    {
        // Updates Player preference to value set by slider

        PlayerPrefs.SetFloat("Master Volume", masterVolumeSlider.value);

        // Updates text box to represent number input by slider
        masterVolumeInput.text = masterVolumeSlider.value.ToString();

    }

    public void UpdateMusicVolumeSliderValue()
    {
        // Updates Player preference to value set by slider

        PlayerPrefs.SetFloat("Music Volume", musicVolumeSlider.value);

        // Updates text box to represent number input by slider
        musicVolumeInput.text = musicVolumeSlider.value.ToString();

    }
    public void UpdateSFXVolumeSliderValue()
    {
        // Updates Player preference to value set by slider

        PlayerPrefs.SetFloat("SFX Volume", sfxVolumeSlider.value);

        // Updates text box to represent number input by slider
        sfxVolumeInput.text = sfxVolumeSlider.value.ToString();

    }

    public void UpdateBrightnessSliderValue()
    {
        // Updates Player preference to value set by slider

        PlayerPrefs.SetFloat("Brightness", brightnessSlider.value);

        // Updates text box to represent number input by slider
        brightnessInput.text = brightnessSlider.value.ToString();
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
