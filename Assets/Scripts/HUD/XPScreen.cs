using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class XPScreen : BaseScreen, IPointerClickHandler
{
    /* XP bar slider */
    [SerializeField] private Slider barSlider;

    /* Display text for if player won or loss the game */
    [SerializeField] private TextMeshProUGUI headerText;

    /* Displays the total xp earned from this run */
    [SerializeField] private TextMeshProUGUI totalXPText;

    /* prefab of enemy text holder */
    [SerializeField] private GameObject enemyUIPrefab;

    /* where all the enemy text holders will go under, their parent */
    [SerializeField] private GameObject contentHolder;

    [SerializeField] [Tooltip("Time it takes to fill the bar up from previous to current total")] private float timeBetweenUIBlocks = 1f;

    /* tracks all the enemy text holders created */
    private List<GameObject> m_EnemyTextHolders = new List<GameObject>();

    /* Bar fill and Ui Block animation speed */
    private float m_UIBlocknAnimationSpeed = 0f;

    private float m_BarAnimationSpeed = 0f;

    /* Previous amount of xp */
    private float m_PreviousXP = 0;

    /* total xp earned this run */
    private int m_TotalXPEarned = 0;

    private int m_CompletedUIBlocks = 0;

    private bool m_AnimationFinished = false;
    private bool m_XPCalculationFinished = false;

    public override void Start()
    {
        base.Start();

        m_UIBlocknAnimationSpeed = 15 / timeBetweenUIBlocks;
    }

    public override void Show()
    {
        base.Show();

        // Disables minimap canvas
        GameManager.Instance.minimapCanvas.SetActive(false);

        m_PreviousXP = 0;
        m_TotalXPEarned = 0;

        GameManager.Instance.IsXPScreenShowing = true;

        // unlock mouse 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Update header text
        if (GameManager.Instance.GameWon)
        {
            headerText.text = "Success";
        }
        else
        {
            headerText.text = "You Died";
        }

        // set the bars value to the previous xp earned from last run
        barSlider.value = GameManager.Instance.PreviousXP / GameManager.Instance.maxXPAmount;

        m_AnimationFinished = false;
        m_XPCalculationFinished = false;

        m_CompletedUIBlocks = 0;

        StartCoroutine(ShowCollectedXP());
    }

    public override void Update()
    {
        base.Update();

        if (m_AnimationFinished) return;

        // Moves all of the UI Blocks down 
        for (int i = 0; i < m_EnemyTextHolders.Count; i++)
        {
            RectTransform trans = m_EnemyTextHolders[i].GetComponent<RectTransform>();

            Vector2 targetPos = trans.anchoredPosition;
            int multiplier = m_EnemyTextHolders.Count - i;
            targetPos.y = trans.rect.height * multiplier * -1 + 30f - (multiplier * 10f);

            trans.anchoredPosition = Vector2.Lerp(trans.anchoredPosition, targetPos, m_UIBlocknAnimationSpeed * Time.deltaTime);
        }

        m_PreviousXP = Mathf.Lerp(m_PreviousXP, GameManager.Instance.CurrentXP + m_TotalXPEarned, m_BarAnimationSpeed * Time.deltaTime);
        barSlider.value = m_PreviousXP / GameManager.Instance.maxXPAmount;
    }

    public override void Hide()
    {
        base.Hide();

        // lock mouse again
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        // clear all holders from scroll view 
        m_EnemyTextHolders.Clear();

        // resets enemy kill counts
        GameManager.Instance.enemiesKilled.Clear();
    }

    public void OnNextButtonClick()
    {
        // hides this screen
        ScreenManager.Instance.HideScreen(screenName);

        // shows shop
        GameManager.Instance.shopCanvas.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_XPCalculationFinished) return;

        m_XPCalculationFinished = true;
        m_AnimationFinished = true;

        StopAllCoroutines();

        int i = 0;

        if (m_CompletedUIBlocks < GameManager.Instance.enemiesKilled.Count)
        {
            foreach (KeyValuePair<string, int> pair in GameManager.Instance.enemiesKilled)
            {
                if (i > (m_CompletedUIBlocks - 1))
                {
                    m_TotalXPEarned += GameManager.Instance.CalculateXPForEnemy(pair.Key);

                    // Adds a new textholder to the scroll view, ands sets its appopriate names and kill amounts
                    EnemyTextHolder enemyTextHolder = AddEnemyTextHolder();
                    enemyTextHolder.enemyNameText.text = pair.Key;
                    enemyTextHolder.enemyKillText.text = "x" + pair.Value;
                }
                i++;
            }
        }

        if (GameManager.Instance.IntelCollected > 0)
        {
            m_TotalXPEarned += GameManager.Instance.IntelCollected * 20;

            EnemyTextHolder enemyTextHolder = AddEnemyTextHolder();
            enemyTextHolder.enemyNameText.text = "Intels";
            enemyTextHolder.enemyKillText.text = "x" + GameManager.Instance.IntelCollected;

            // resets player intel collected count
            GameManager.Instance.IntelCollected = 0;
        }

        for (i = 0; i < m_EnemyTextHolders.Count; i++)
        {
            RectTransform trans = m_EnemyTextHolders[i].GetComponent<RectTransform>();

            Vector2 targetPos = trans.anchoredPosition;
            int multiplier = m_EnemyTextHolders.Count - i;
            targetPos.y = trans.rect.height * multiplier * -1 + 30f - (multiplier * 10f);

            trans.anchoredPosition = targetPos;
        }

        // Updates total xp earned text
        totalXPText.text = "Total XP: " + m_TotalXPEarned;
        GameManager.Instance.CurrentXP += m_TotalXPEarned;
        barSlider.value = (float)GameManager.Instance.CurrentXP / GameManager.Instance.maxXPAmount;
    }

    /* 
     * Shows all the xp collected from every thing
     */
    IEnumerator ShowCollectedXP()
    {
        foreach (KeyValuePair<string, int> pair in GameManager.Instance.enemiesKilled)
        {
            m_PreviousXP = GameManager.Instance.PreviousXP + m_TotalXPEarned;

            m_TotalXPEarned += GameManager.Instance.CalculateXPForEnemy(pair.Key);

            // Adds a new textholder to the scroll view, ands sets its appopriate names and kill amounts
            EnemyTextHolder enemyTextHolder = AddEnemyTextHolder();
            enemyTextHolder.enemyNameText.text = pair.Key;
            enemyTextHolder.enemyKillText.text = "x" + pair.Value;

            // Updates total xp earned text
            totalXPText.text = "Total XP: " + m_TotalXPEarned;

            m_BarAnimationSpeed = (m_TotalXPEarned - m_PreviousXP) / timeBetweenUIBlocks;

            m_CompletedUIBlocks++;

            yield return new WaitForSeconds(timeBetweenUIBlocks);
        }

        if (GameManager.Instance.IntelCollected > 0)
        {
            m_PreviousXP = GameManager.Instance.PreviousXP + m_TotalXPEarned;

            m_TotalXPEarned += GameManager.Instance.IntelCollected * 20;

            //barSlider.value = (float)GameManager.Instance.CurrentXP / GameManager.Instance.maxXPAmount;

            EnemyTextHolder enemyTextHolder = AddEnemyTextHolder();
            enemyTextHolder.enemyNameText.text = "Intels";
            enemyTextHolder.enemyKillText.text = "x" + GameManager.Instance.IntelCollected;

            // resets player intel collected count
            GameManager.Instance.IntelCollected = 0;

            // Updates total xp earned text
            totalXPText.text = "Total XP: " + m_TotalXPEarned;

            yield return new WaitForSeconds(timeBetweenUIBlocks);
        }

        GameManager.Instance.CurrentXP += m_TotalXPEarned;

        m_AnimationFinished = true;
        m_XPCalculationFinished = true;
    }

    EnemyTextHolder AddEnemyTextHolder()
    {
        GameObject enemytextHolder = Instantiate(enemyUIPrefab, contentHolder.transform);
        m_EnemyTextHolders.Add(enemytextHolder);

        return enemytextHolder.GetComponent<EnemyTextHolder>();
    }
}
