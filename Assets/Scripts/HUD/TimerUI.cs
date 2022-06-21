using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    private static TimerUI mInstance;
    public static TimerUI Instance { get { return mInstance; } }

    private TextMeshProUGUI mText;

    private bool bIsShowing = true;

    private float mTime = 0f;
    private int mSeconds = 0;
    private float mTimeStamp = 0f;
    // Start is called before the first frame update

    private void Awake()
    {
        if (mInstance == null)
            mInstance = this;
    }

    void Start()
    {
        mText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        mTime += Time.fixedDeltaTime;

        mSeconds = (int)(mTime % 60);
        if (mSeconds > 9)
        {
            mText.text = ((int)(mTime / 60f)).ToString() + ":" + mSeconds.ToString();
        }
        else
        {
            mText.text = ((int)(mTime / 60f)).ToString() + ":0" + mSeconds.ToString();
        }
    }

    public void Toggle()
    {
        bIsShowing = !bIsShowing;
        gameObject.SetActive(bIsShowing);
    }

    private void TimeStamp()
    {
        mTimeStamp = mTime;
    }

    public float TimePastFromTimeStamp()
    {
        float deltaTime = mTime - mTimeStamp;
        TimeStamp();

        return deltaTime;
    }

    public void ResetTimer()
    {
        mTime = 0f;
        mTimeStamp = 0f;
    }

    public int GetElapsedTime()
    {
        return (int)mTime;
    }
}
