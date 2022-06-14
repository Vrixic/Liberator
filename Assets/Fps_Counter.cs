using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Fps_Counter : MonoBehaviour
{
    TextMeshProUGUI m_FPSText;

    private void Start()
    {
        m_FPSText = GetComponent<TextMeshProUGUI>();
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
}
