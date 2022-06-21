using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private Image splashImage;
    [SerializeField] private float minWaitTime = 2f;

    private AsyncOperation mAsyncOperation;

    // Start is called before the first frame update
    void Start()
    {
        mAsyncOperation = SceneManager.LoadSceneAsync("MainMenuScene");
        mAsyncOperation.allowSceneActivation = false;

        StartCoroutine(Wait());
    }

   private IEnumerator Wait()
   {
        float time = 0f;

        while(time < 2f)
        {
            time += Time.deltaTime;

            yield return null;
        }

        mAsyncOperation.allowSceneActivation = true;
   }
}
