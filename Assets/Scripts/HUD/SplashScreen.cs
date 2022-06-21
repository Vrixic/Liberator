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
        Color color = splashImage.color;

        while(time < minWaitTime)
        {
            time += Time.deltaTime;
            color.a = Mathf.Sin(Time.time);

            splashImage.color = color;

            yield return null;
        }

        mAsyncOperation.allowSceneActivation = true;
   }
}
