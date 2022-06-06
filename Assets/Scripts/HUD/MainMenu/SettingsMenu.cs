using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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

    private void OnEnable()
    {
        // Master Volume
        if (PlayerPrefs.HasKey("Master Volume"))
        {
            PlayerPrefManager.Instance.masterVolume = PlayerPrefs.GetFloat("Master Volume", 100f);
        }
        else
        {
            PlayerPrefManager.Instance.masterVolume = 100f;
            PlayerPrefs.SetFloat("Master Volume", PlayerPrefManager.Instance.masterVolume);
        }

        // Music Volume
        if (PlayerPrefs.HasKey("Music Volume"))
        {
            PlayerPrefManager.Instance.musicVolume = PlayerPrefs.GetFloat("Music Volume", 100f);
        }
        else
        {
            PlayerPrefManager.Instance.musicVolume = 100f;
            PlayerPrefs.SetFloat("Music Volume", PlayerPrefManager.Instance.musicVolume);
        }

        // SFX volume
        if (PlayerPrefs.HasKey("SFX Volume"))
        {
            PlayerPrefManager.Instance.sfxVolume = PlayerPrefs.GetFloat("SFX Volume", 100f);
        }
        else
        {
            PlayerPrefManager.Instance.sfxVolume = 100f;
            PlayerPrefs.SetFloat("SFX Volume", PlayerPrefManager.Instance.sfxVolume);
        }

        // Player Sensitivity
        if (PlayerPrefs.HasKey("Player Sensitivity"))
        {
            PlayerPrefManager.Instance.playerSensitivity = PlayerPrefs.GetFloat("Player Sensitivity", 100f);
        }
        else
        {
            PlayerPrefManager.Instance.playerSensitivity = 100f;
            PlayerPrefs.SetFloat("Player Sensitivity", PlayerPrefManager.Instance.playerSensitivity);
        }

        // Brightness
        if (PlayerPrefs.HasKey("Brightness"))
        {
            PlayerPrefManager.Instance.brightness = PlayerPrefs.GetFloat("Brightness", 100f);
        }
        else
        {
            PlayerPrefManager.Instance.brightness = 100f;
            PlayerPrefs.SetFloat("Brightness", PlayerPrefManager.Instance.brightness);
        }

        InitSliders();
    }


    // Initialize Sliders and text boxes to current player prefs
    public void InitSliders()
    {
        masterVolumeInput.text = PlayerPrefManager.Instance.masterVolume.ToString();
        masterVolumeSlider.value = PlayerPrefManager.Instance.masterVolume;

        musicVolumeInput.text = PlayerPrefManager.Instance.musicVolume.ToString();
        musicVolumeSlider.value = PlayerPrefManager.Instance.musicVolume;

        sfxVolumeInput.text = PlayerPrefManager.Instance.sfxVolume.ToString();
        sfxVolumeSlider.value = PlayerPrefManager.Instance.sfxVolume;

        sensitivityInput.text = PlayerPrefManager.Instance.playerSensitivity.ToString();
        sensitivitySlider.value = PlayerPrefManager.Instance.playerSensitivity;

        brightnessInput.text = PlayerPrefManager.Instance.brightness.ToString();
        brightnessSlider.value = PlayerPrefManager.Instance.brightness;
        Debug.Log("Settings Sliders Initialized");
    }

    #region Update Sliders and Text Fields Methods
    public void UpdateMasterVolumeInputValue()
    {
        // If player inputs "00" or "000" it defaults to only show one 0
        if (masterVolumeInput.text == "00" || masterVolumeInput.text == "000")
        {
            masterVolumeInput.text = "0";
        }
        // Updates all volume preferences to represent what master volume is set to in text box


        PlayerPrefs.SetFloat("Master Volume", int.Parse(masterVolumeInput.text));
        PlayerPrefs.SetFloat("Music Volume", int.Parse(masterVolumeInput.text));
        PlayerPrefs.SetFloat("SFX Volume", int.Parse(masterVolumeInput.text));

        // Updates Slider to represent value inputted into text box
        masterVolumeSlider.value = int.Parse(masterVolumeInput.text);
        musicVolumeSlider.value = int.Parse(masterVolumeInput.text);
        sfxVolumeSlider.value = int.Parse(masterVolumeInput.text);

        PlayerPrefManager.Instance.masterVolume = masterVolumeSlider.value;
        //PlayerPrefManager.Instance.musicVolume = masterVolumeSlider.value;
        //PlayerPrefManager.Instance.sfxVolume = masterVolumeSlider.value;
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

        PlayerPrefManager.Instance.musicVolume = musicVolumeSlider.value;

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

        PlayerPrefManager.Instance.sfxVolume = sfxVolumeSlider.value;


        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            AudioManager.Instance.PlayAudioAtLocation(Vector3.zero, "TestSFX");
        }
        else
        {
            AudioManager.Instance.PlayAudioAtLocation(Vector3.zero, "TestSFX");

        }

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

        PlayerPrefManager.Instance.brightness = brightnessSlider.value;

        RenderSettings.ambientLight = new Color(PlayerPrefManager.Instance.brightness / 100, PlayerPrefManager.Instance.brightness / 100, PlayerPrefManager.Instance.brightness / 100, 1.0f);

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

        PlayerPrefManager.Instance.playerSensitivity = sensitivitySlider.value;

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            GameManager.Instance.playerLookScript.OnSensitivityUpdate();
        }
    }

    public void UpdateMasterVolumeSliderValue()
    {
        // Updates all volume preferences to represent what master volume is set to

        PlayerPrefs.SetFloat("Master Volume", masterVolumeSlider.value);
        PlayerPrefs.SetFloat("Music Volume", masterVolumeSlider.value);
        PlayerPrefs.SetFloat("SFX Volume", masterVolumeSlider.value);

        // Updates text box to represent number input by slider
        masterVolumeInput.text = masterVolumeSlider.value.ToString();
        musicVolumeInput.text = masterVolumeSlider.value.ToString();
        sfxVolumeInput.text = masterVolumeSlider.value.ToString();

        PlayerPrefManager.Instance.masterVolume = masterVolumeSlider.value;
        //PlayerPrefManager.Instance.musicVolume = masterVolumeSlider.value;
        //PlayerPrefManager.Instance.sfxVolume = masterVolumeSlider.value;
    }

    public void UpdateMusicVolumeSliderValue()
    {
        // Updates Player preference to value set by slider

        PlayerPrefs.SetFloat("Music Volume", musicVolumeSlider.value);

        // Updates text box to represent number input by slider
        musicVolumeInput.text = musicVolumeSlider.value.ToString();

        PlayerPrefManager.Instance.musicVolume = musicVolumeSlider.value;
    }

    public void UpdateSFXVolumeSliderValue()
    {
        // Updates Player preference to value set by slider

        PlayerPrefs.SetFloat("SFX Volume", sfxVolumeSlider.value);

        // Updates text box to represent number input by slider
        sfxVolumeInput.text = sfxVolumeSlider.value.ToString();

        PlayerPrefManager.Instance.sfxVolume = sfxVolumeSlider.value;

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            AudioManager.Instance.PlayAudioAtLocation(Vector3.zero, "TestSFX");
        }
        else
        {
            AudioManager.Instance.PlayAudioAtLocation(Vector3.zero, "TestSFX");

        }
    }

    public void UpdateBrightnessSliderValue()
    {
        // Updates Player preference to value set by slider

        PlayerPrefs.SetFloat("Brightness", brightnessSlider.value);

        // Updates text box to represent number input by slider
        brightnessInput.text = brightnessSlider.value.ToString();

        PlayerPrefManager.Instance.brightness = brightnessSlider.value;

        RenderSettings.ambientLight = new Color(PlayerPrefManager.Instance.brightness / 100, PlayerPrefManager.Instance.brightness / 100, PlayerPrefManager.Instance.brightness / 100, 1.0f);
    }


    public void UpdateSensitivitySliderValue()
    {
        // Updates Player preference to value set by slider

        PlayerPrefs.SetFloat("Player Sensitivity", sensitivitySlider.value);

        // Updates text box to represent number input by slider
        sensitivityInput.text = sensitivitySlider.value.ToString();

        PlayerPrefManager.Instance.playerSensitivity = sensitivitySlider.value;

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            GameManager.Instance.playerLookScript.OnSensitivityUpdate();
        }

    }

    #endregion
}
