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
        Time.timeScale = 0.2f;
        yield return new WaitForSeconds(slowTimeLength* 0.2f);
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
