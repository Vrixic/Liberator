using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TransitionScreen : BaseScreen, IPointerClickHandler
{
    /* progress bar slider */
    [SerializeField] private Slider barSlider;

    [SerializeField] private float loadTimeMin = 1f;

    private float m_LoadEndTimeMin = 0f;
    private bool bSceneLoading = false;

    public override void Show()
    {
        base.Show();

        GameManager.Instance.SceneOperation.completed += OnSceneLoaded;
        m_LoadEndTimeMin = Time.realtimeSinceStartup + loadTimeMin;        
    }

    public override void Update()
    {
        if(Time.realtimeSinceStartup < m_LoadEndTimeMin)
        {
            barSlider.value = GameManager.Instance.SceneOperation.progress - (m_LoadEndTimeMin - Time.realtimeSinceStartup) / loadTimeMin;
        }
        else 
        {
            GameManager.Instance.SceneOperation.allowSceneActivation = true;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(GameManager.Instance.SceneOperation.progress > 0.89 && !bSceneLoading)
        {
            bSceneLoading = true;
            GameManager.Instance.SceneOperation.allowSceneActivation = true;
        }
    }

    void OnSceneLoaded(AsyncOperation operation)
    {       
        ScreenManager.Instance.HideScreen(screenName);
    }
}
