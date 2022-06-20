using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InformationPopup : MonoBehaviour
{
    [SerializeField] string popUpText = "";
    [SerializeField] bool textPrompt = true;
    GameObject informationPrompt;
    AudioSource audioSource;

    bool bAudioIsPlaying = false;

    bool bPaused = false;

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
        else if (PlayerPrefManager.Instance.voicePromptState == 1)
        {
            ResumeAllPrompts();
        }
        if (Time.timeScale < 0.01f)
        {
            bPaused = true;
            informationPrompt.gameObject.SetActive(false);
        }
        if (bPaused && Time.timeScale > 0)
        {
            bPaused = false;
            informationPrompt.gameObject.SetActive(true);
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
        yield return new WaitForSeconds(3f);
        informationPrompt.SetActive(false);
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
        gameObject.SetActive(false);
        audioSource.Stop();
        bAudioIsPlaying = false;
    }

    void ResumeAllPrompts()
    {
        gameObject.SetActive(true);
    }
}
