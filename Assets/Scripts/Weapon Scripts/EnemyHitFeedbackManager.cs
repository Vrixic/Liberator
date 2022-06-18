using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHitFeedbackManager : MonoBehaviour
{

    public static EnemyHitFeedbackManager Instance { get { return mInstance; } }
    private static EnemyHitFeedbackManager mInstance;

    private Image mFeedbackImage;
    private Color mImageColor;

    private bool bIsFading = false;

    // Start is called before the first frame update
    void Start()
    {
        if (mInstance == null)
            mInstance = this;

        mFeedbackImage = GetComponentInChildren<Image>();
        mImageColor = mFeedbackImage.color;

        mFeedbackImage.gameObject.SetActive(false);
    }

    public void ShowHitFeedback(Color color)
    {
        mImageColor = color;
        mFeedbackImage.color = mImageColor;

        if(!bIsFading)
            StartCoroutine(FadeOutFeedbackImage());
    }

    /* 
     * Fades out the currently showing gun icon
     */
    IEnumerator FadeOutFeedbackImage()
    {
        mFeedbackImage.gameObject.SetActive(true);
        bIsFading = true;

        for (float alpha = mImageColor.a; alpha >= 0; alpha = mImageColor.a)
        {
            alpha -= Time.deltaTime * 5f;

            mImageColor.a = alpha;
            mFeedbackImage.color = mImageColor;

            yield return null;
        }

        mFeedbackImage.gameObject.SetActive(false);
        bIsFading = false;
    }
}
