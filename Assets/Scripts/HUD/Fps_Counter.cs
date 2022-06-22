using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Fps_Counter : MonoBehaviour
{
    TextMeshProUGUI m_FPSText;
    bool bLastState = false;

    private void Start()
    {
        m_FPSText = GetComponent<TextMeshProUGUI>();
        m_FPSText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        m_FPSText.text = ((int)(1 / Time.unscaledDeltaTime)).ToString();
    }

    public void ToggleCounterDisplay()
    {
        m_FPSText.enabled = !m_FPSText.enabled;
    }

    public void Hide()
    {
        bLastState = m_FPSText.enabled;
        m_FPSText.enabled = false;
    }

    public void SetToLastState()
    {
        m_FPSText.enabled = bLastState;
    }
}
