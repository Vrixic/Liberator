using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private Image teamImage;
    [SerializeField] private Image gameImage;
    [SerializeField] private float minWaitTime = 2f;

    private AsyncOperation mAsyncOperation;

    // Start is called before the first frame update
    void Start()
    {
        mAsyncOperation = SceneManager.LoadSceneAsync("MainMenuScene");
        mAsyncOperation.allowSceneActivation = false;

        gameImage.gameObject.SetActive(false);

        StartCoroutine(FadeInOutTeamLogo());
    }

   private IEnumerator FadeInOutTeamLogo()
   {
        float time = 0f;
        Color color = teamImage.color;

        while(time < minWaitTime)
        {
            time += Time.deltaTime;
            color.a = Mathf.Sin(time);

            teamImage.color = color;

            yield return null;
        }

        teamImage.gameObject.SetActive(false);
        StartCoroutine(FadeInOutGameLogo());
   }

    private IEnumerator FadeInOutGameLogo()
    {
        gameImage.gameObject.SetActive(true);

        float time = 0f;
        Color color = gameImage.color;

        while (time < minWaitTime)
        {
            time += Time.deltaTime;
            color.a = Mathf.Sin(time);

            gameImage.color = color;

            yield return null;
        }

        mAsyncOperation.allowSceneActivation = true;
    }
}
