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

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        informationPrompt = GameManager.Instance.infomationTriggerText;
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            GetComponent<BoxCollider>().enabled = false;
            informationPrompt.GetComponent<TMP_Text>().text = popUpText;
            if (slowTime)
            {
                StartCoroutine(ShowSlow());
            }
            else
            {
                StartCoroutine(ShowNormal());
            }
        }
    }

    IEnumerator ShowSlow()
    {
        informationPrompt.SetActive(true);
        Time.timeScale = 0.2f;
        yield return new WaitForSeconds(slowTimeLength* 0.2f);
        informationPrompt.SetActive(false);
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    IEnumerator ShowNormal()
    {
        informationPrompt.SetActive(true);
        audioSource.Play();
        yield return new WaitForSeconds(20);
        informationPrompt.SetActive(false);
        gameObject.SetActive(false);
    }
}
