using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InformationPopup : MonoBehaviour
{
    [SerializeField] string popUpText = "";
    [SerializeField] bool slowTime = true;
    GameObject informationPrompt;

    private void Start()
    {
        informationPrompt = GameManager.Instance.infomationTriggerText;
    }
    private void OnTriggerEnter(Collider other)
    {
        informationPrompt.GetComponent<TMP_Text>().text = popUpText;
        if (other.CompareTag("Player"))
        {
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
        float time = 0f;
        Time.timeScale = 0.1f;
        while (Time.timeScale < 0.6)
        {
            time += 0.2f * Time.unscaledDeltaTime;
            time = Mathf.Clamp(time, 0f, 1f);
            Time.timeScale = time;
            yield return null;
        }
        informationPrompt.SetActive(false);
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    IEnumerator ShowNormal()
    {
        informationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        informationPrompt.SetActive(false);
        gameObject.SetActive(false);
    }
}
