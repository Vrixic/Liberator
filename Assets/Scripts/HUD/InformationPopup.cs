using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InformationPopup : MonoBehaviour
{
    [SerializeField] string popUpText = "";
    [SerializeField] bool slowTime = true;
    [SerializeField] float slowTimeLength = 3f;
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
        if (Time.timeScale < 0.01f)
        {
            bPaused = true;
        }
        if (bPaused && Time.timeScale > 0)
        {
            bPaused = false;
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
            if (slowTime)
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
        Time.timeScale = 0.2f;
        yield return new WaitForSeconds(slowTimeLength* 0.2f);
        informationPrompt.SetActive(false);
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    IEnumerator VoicePrompt()
    {
        bAudioIsPlaying = true;
        informationPrompt.SetActive(true);
        audioSource.Play();
        yield return new WaitForSeconds(20);
        informationPrompt.SetActive(false);
        gameObject.SetActive(false);
    }


    void StopAllPrompts()
    {
        StopCoroutine(TextPromptSlow());
        StopCoroutine(VoicePrompt());
        Time.timeScale = 1;
        informationPrompt.SetActive(false);
        gameObject.SetActive(false);
        audioSource.Stop();
        bAudioIsPlaying = false;
    }
}
