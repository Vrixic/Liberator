using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonFunctionality : MonoBehaviour
{
    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    #region MainMenu

    // REFERENCES FOR SCENE INDICES
    // Main menu Scene index = 0 & Game scene index = 1
    public void StartGame()
    {

        // Loads the game Scene
        SceneManager.LoadScene(1);

    }

    public void OpenOptionsMenu()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Application is Exiting");
    }
    #endregion

}
