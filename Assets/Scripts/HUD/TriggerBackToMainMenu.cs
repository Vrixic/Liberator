using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerBackToMainMenu : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPrefManager.Instance.SceneOperation = SceneManager.LoadSceneAsync("MainMenuScene");
            PlayerPrefManager.Instance.SceneOperation.allowSceneActivation = false;
            ScreenManager.Instance.ShowScreen("Transition_Screen");
        }
    }
}
