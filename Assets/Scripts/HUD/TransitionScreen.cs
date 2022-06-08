using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TransitionScreen : BaseScreen, IPointerClickHandler
{
    /* progress bar slider */
    [SerializeField] private Slider barSlider;

    [SerializeField] private float loadTimeMin = 1f;

    [SerializeField] private List<Sprite> backgroundImagesList;

    [SerializeField] private TextMeshProUGUI loadedText;

    [SerializeField] private List<string> loadingTips;

    private Image m_BackgroundImage;

    private float m_LoadEndTimeMin = 0f;
    private bool bSceneLoading = false;

    public override void Start()
    {
        base.Start();

        m_BackgroundImage = GetComponentInChildren<Image>();
    }

    public override void Show()
    {
        base.Show();

        Random.InitState(Time.frameCount);
        m_BackgroundImage.sprite = backgroundImagesList[Random.Range(0, backgroundImagesList.Count)];
        Random.InitState(Time.frameCount);
        loadedText.text = loadingTips[Random.Range(0, loadingTips.Count)];

        PlayerPrefManager.Instance.SceneOperation.completed += OnSceneLoaded;
        m_LoadEndTimeMin = Time.realtimeSinceStartup + loadTimeMin;

        bSceneLoading = false;
    }

    public override void Update()
    {
        if(Time.realtimeSinceStartup < m_LoadEndTimeMin)
        {
            barSlider.value = PlayerPrefManager.Instance.SceneOperation.progress - (m_LoadEndTimeMin - Time.realtimeSinceStartup) / loadTimeMin;
        }
        else 
        {
            PlayerPrefManager.Instance.SceneOperation.allowSceneActivation = true;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(PlayerPrefManager.Instance.SceneOperation.progress > 0.89 && !bSceneLoading)
        {
            bSceneLoading = true;
            PlayerPrefManager.Instance.SceneOperation.allowSceneActivation = true;
        }
    }

    void OnSceneLoaded(AsyncOperation operation)
    {
        ScreenManager.Instance.HideScreen(screenName);
    }
}
