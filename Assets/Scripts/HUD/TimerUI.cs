using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    private static TimerUI mInstance;
    public static TimerUI Instance { get { return mInstance; } }

    private TextMeshProUGUI mText;

    private int mSeconds = 0;
    // Start is called before the first frame update

    private void Awake()
    {
        if (mInstance != this)
            mInstance = this;
    }

    void Start()
    {
        mText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        mSeconds = (int)(Time.time % 60);
        if (mSeconds > 10)
        {
            mText.text = ((int)(Time.time / 60f)).ToString() + ":" + mSeconds.ToString();
        }
        else
        {
            mText.text = ((int)(Time.time / 60f)).ToString() + ":0" + mSeconds.ToString();
        }
        
    }

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
        //if (gameObject.activeSelf)
        //    Hide();
        //else
        //    Show();
    }

    private void Show()
    {
        gameObject.SetActive(false);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
