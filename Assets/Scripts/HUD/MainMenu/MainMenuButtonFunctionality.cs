using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuButtonFunctionality : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlayAudioAtLocation(Vector3.zero, "ButtonPress");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlayAudioAtLocation(Vector3.zero, "ButtonHover");
    }

    #region MainMenu

    // REFERENCES FOR SCENE INDICES
    // Main menu Scene index = 0 & Game scene index = 1
    public void StartGame()
    {

        // Loads the game Scene
        Debug.Log("Starting Game From Main Menu");
        PlayerPrefManager.Instance.LoadGame();

        PlayerPrefManager.Instance.SceneOperation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        PlayerPrefManager.Instance.SceneOperation.allowSceneActivation = false;
        ScreenManager.Instance.ShowScreen("Transition_Screen");

        //pauses menu music
        AudioManager.Instance.StopMusic();

        //starts game music
        Invoke("PlayGameMusicInvoked", 2);

        //SceneManager.LoadScene(1);
        // Sets cursor state to locked and turns off the visibility
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Resumes time
        Time.timeScale = 1f;

    }


    public void QuitGame()
    {
        Debug.Log("Application is Exiting");
        PlayerPrefManager.Instance.SaveGame();
        Application.Quit();
    }

    public void PlayGameMusicInvoked()
    {
        AudioManager.Instance.PlayAudioAtLocation(Vector3.zero, "GameMusic");
    }

    // TO DO: IMPLEMENT Tutorial
    public void LoadTutorial()
    {

    }

    #endregion

   


}

