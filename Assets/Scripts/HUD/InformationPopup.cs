using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class InformationPopup : MonoBehaviour
{
    [SerializeField] string popUpText = "";
    [SerializeField] bool textPrompt = true;
    GameObject informationPrompt;
    AudioSource audioSource;

    bool bAudioIsPlaying = false;

    bool bPaused = false;

    bool bIsKeyPressed = false;

    [SerializeField] string actionToDo = "";

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        informationPrompt = GameManager.Instance.infomationTriggerText;
    }
    private void Update()
    {
        if (PlayerPrefManager.Instance.voicePromptState == 0)
        {
            StopAllPrompts();
        }
        if (GameManager.Instance.isPauseMenuOpen)
        {
            bPaused = true;
            informationPrompt.SetActive(false);
        }
        if (bPaused && Time.timeScale > 0)
        {
            bPaused = false;
            informationPrompt.SetActive(true);
        }
        if (bAudioIsPlaying)
        {
            audioSource.volume = PlayerPrefManager.Instance.sfxVolume / 100;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PlayerPrefManager.Instance.voicePromptState == 0) return;
        if (other.CompareTag("Player"))
        {
            GetComponent<BoxCollider>().enabled = false;
            informationPrompt.GetComponent<TMP_Text>().text = popUpText;
            if (textPrompt)
            {
                StartCoroutine(TextPromptSlow());
            }
            else
            {
                StartCoroutine(VoicePrompt());
            }
        }
    }

    IEnumerator TextPromptSlow()
    {
        informationPrompt.SetActive(true);
        if (actionToDo != "")
        {
            while (!bIsKeyPressed)
            {
                if (InputManager.Instance.playerInput.FindAction(actionToDo).triggered && !GameManager.Instance.isPauseMenuOpen)
                {
                    bIsKeyPressed = true;
                }
                Time.timeScale = 0.0001f;

                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(4);
        }
        
        informationPrompt.SetActive(false);
        Time.timeScale = 1;
        bIsKeyPressed = false;
        gameObject.SetActive(false);
    }

    IEnumerator VoicePrompt()
    {
        float timeToPlay = 20;
        bAudioIsPlaying = true;
        informationPrompt.SetActive(true);
        audioSource.Play();
        while (timeToPlay > 0)
        {
            if (!bPaused)
            {
                timeToPlay -= Time.deltaTime;
                audioSource.UnPause();
                informationPrompt.SetActive(true);
            }
            if (bPaused)
            {
                audioSource.Pause();
                informationPrompt.SetActive(false);
            }
            yield return null;
        }
        informationPrompt.SetActive(false);
        gameObject.SetActive(false);
    }


    void StopAllPrompts()
    {
        StopCoroutine(TextPromptSlow());
        StopCoroutine(VoicePrompt());
        informationPrompt.SetActive(false);
        audioSource.Stop();
        bAudioIsPlaying = false;
    }
}
