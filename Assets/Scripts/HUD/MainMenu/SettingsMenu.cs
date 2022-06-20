using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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
    public Toggle voicePromptToggle;
    bool dontUpdate = false;
    private void OnEnable()
    {

        EventSystem.current.SetSelectedGameObject(GetComponentInChildren<Button>().gameObject);
        PlayerPrefManager.Instance.LoadSettings();
        InitSliders();
    }



    // Initialize Sliders and text boxes to current player prefs
    public void InitSliders()
    {
        // Set dont update bool to true to prevent master volume from changing other sliders when settings menu is entered for first time
        dontUpdate = true;
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

        if (PlayerPrefManager.Instance.voicePromptState == 1)
        {
            voicePromptToggle.isOn = true;
        }
        else if(PlayerPrefManager.Instance.voicePromptState == 0)
        {
            voicePromptToggle.isOn = false;
        }

        dontUpdate = false;
    }
    public void OnUpdateVoicePromptToggle()
    {
        if (!voicePromptToggle.isOn)
        {
            PlayerPrefs.SetInt("Voice Prompts State", 1);
            PlayerPrefManager.Instance.voicePromptState = 1;
        }
        else if (voicePromptToggle.isOn)
        {
            PlayerPrefs.SetInt("Voice Prompts State", 0);
            PlayerPrefManager.Instance.voicePromptState = 0;
        }

    }

    #region Update Sliders and Text Fields Methods
    public void UpdateMasterVolumeInputValue()
    {
        // If player inputs "00" or "000" it defaults to only show one 0
        if (masterVolumeInput.text == "00" || masterVolumeInput.text == "000")
        {
            masterVolumeInput.text = "0";
        }

        // Updates master volume preferences
        PlayerPrefs.SetFloat("Master Volume", int.Parse(masterVolumeInput.text));
        masterVolumeSlider.value = int.Parse(masterVolumeInput.text);
        PlayerPrefManager.Instance.masterVolume = masterVolumeSlider.value;

        // Updates all volume preferences to represent what master volume is set to in text box
        if (!dontUpdate)
        {
            PlayerPrefs.SetFloat("Music Volume", int.Parse(masterVolumeInput.text));
            PlayerPrefs.SetFloat("SFX Volume", int.Parse(masterVolumeInput.text));

            // Updates Slider to represent value inputted into text box
            musicVolumeSlider.value = int.Parse(masterVolumeInput.text);
            sfxVolumeSlider.value = int.Parse(masterVolumeInput.text);

            PlayerPrefManager.Instance.musicVolume = masterVolumeSlider.value;
            PlayerPrefManager.Instance.sfxVolume = masterVolumeSlider.value;
        }

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
            //AudioManager.Instance.PlayAudioAtLocation(Vector3.zero, "TestSFX");
        }
        else
        {
            //AudioManager.Instance.PlayAudioAtLocation(Vector3.zero, "TestSFX");

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

        RenderSettings.ambientLight = new Color(PlayerPrefManager.Instance.brightness / 100 + .3f, PlayerPrefManager.Instance.brightness / 100 + .3f, PlayerPrefManager.Instance.brightness / 100 + .3f, 1.0f);

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
        if (!dontUpdate)
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                GameManager.Instance.playerLookScript.OnSensitivityUpdate();
            }
        }
    }

    public void UpdateMasterVolumeSliderValue()
    {

        // Updates master volume preferences
        PlayerPrefs.SetFloat("Master Volume", masterVolumeSlider.value);
        masterVolumeInput.text = masterVolumeSlider.value.ToString();
        PlayerPrefManager.Instance.masterVolume = masterVolumeSlider.value;

        // Updates all volume preferences to represent what master volume is set
        if (!dontUpdate)
        {
            PlayerPrefs.SetFloat("Music Volume", masterVolumeSlider.value);
            PlayerPrefs.SetFloat("SFX Volume", masterVolumeSlider.value);

            // Updates text box to represent number input by slider
            musicVolumeInput.text = masterVolumeSlider.value.ToString();
            sfxVolumeInput.text = masterVolumeSlider.value.ToString();

            PlayerPrefManager.Instance.musicVolume = masterVolumeSlider.value;
            PlayerPrefManager.Instance.sfxVolume = masterVolumeSlider.value;
        }

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
    }

    public void UpdateBrightnessSliderValue()
    {
        // Updates Player preference to value set by slider

        PlayerPrefs.SetFloat("Brightness", brightnessSlider.value);

        // Updates text box to represent number input by slider
        brightnessInput.text = brightnessSlider.value.ToString();

        PlayerPrefManager.Instance.brightness = brightnessSlider.value;

        RenderSettings.ambientLight = new Color(PlayerPrefManager.Instance.brightness / 100 + .3f, PlayerPrefManager.Instance.brightness / 100 + .3f, PlayerPrefManager.Instance.brightness / 100 + .3f, 1.0f);

    }


    public void UpdateSensitivitySliderValue()
    {
        // Updates Player preference to value set by slider

        PlayerPrefs.SetFloat("Player Sensitivity", sensitivitySlider.value);

        // Updates text box to represent number input by slider
        sensitivityInput.text = sensitivitySlider.value.ToString();

        PlayerPrefManager.Instance.playerSensitivity = sensitivitySlider.value;
        if (!dontUpdate)
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                GameManager.Instance.playerLookScript.OnSensitivityUpdate();
            }

        }

    }

    #endregion


}
