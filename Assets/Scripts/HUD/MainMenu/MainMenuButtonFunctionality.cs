using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuButtonFunctionality : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private void OnEnable()
    {
        SetSelectedGameObject();
    }

    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.Play2dAudioOnce("ButtonPress");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.Play2dAudioOnce("ButtonHover");
    }

    public void SetSelectedGameObject()
    {

        EventSystem.current.SetSelectedGameObject(GetComponentInChildren<Button>().gameObject);
    }

    #region MainMenu

    // REFERENCES FOR SCENE INDICES
    // Main menu Scene index = 0 & Game scene index = 1
    public void StartGame()
    {
        // Loads the game Scene
        ScreenManager.Instance.ShowScreen("Transition_Screen");
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            PlayerPrefManager.Instance.SceneOperation = SceneManager.LoadSceneAsync("Level_One_WBGL");
        }
        else
        {
            PlayerPrefManager.Instance.SceneOperation = SceneManager.LoadSceneAsync("Level_One");
        }

        PlayerPrefManager.Instance.SceneOperation.allowSceneActivation = false;

        PlayerPrefManager.Instance.LoadGame();

        //pauses menu music
        AudioManager.Instance.StopMusic();

        // initialize game music, and pause it
        AudioManager.Instance.PlayAudioAtLocation(Vector3.zero, "GameMusic");
        AudioManager.Instance.PauseMusic();

        //SceneManager.LoadScene(1);
        // Sets cursor state to locked and turns off the visibility
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Resumes time
        Time.timeScale = 1f;

    }


    public void QuitGame()
    {
        PlayerPrefManager.Instance.SaveGame();

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Application.OpenURL("https://vrixic.itch.io/liberator");
        }
        else
        {

            Application.Quit();
        }


    }

    public void LoadTutorial()
    {
        ScreenManager.Instance.ShowScreen("Transition_Screen");
        PlayerPrefManager.Instance.SceneOperation = SceneManager.LoadSceneAsync("TutorialScene", LoadSceneMode.Single);
        PlayerPrefManager.Instance.SceneOperation.allowSceneActivation = false;

        PlayerPrefManager.Instance.LoadGame();

        //pauses menu music
        AudioManager.Instance.StopMusic();

        // initialize game music, and pause it
        AudioManager.Instance.PlayAudioAtLocation(Vector3.zero, "GameMusic");
        AudioManager.Instance.PauseMusic();

        // Sets cursor state to locked and turns off the visibility
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Resumes time
        Time.timeScale = 1f;
    }

    public void ShowLeaderboard()
    {
        ScreenManager.Instance.ShowScreen("Leaderboard_Screen");
    }

    #endregion




}

