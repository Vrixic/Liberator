using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPScreen : BaseScreen
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

    /* tracks all the enemy text holders created */
    private List<GameObject> m_EnemyTextHolders = new List<GameObject>();

    /* total xp earned this run */
    private int m_TotalXPEarned = 0;

    public override void Show()
    {
        base.Show();

        // Disables minimap canvas
        GameManager.Instance.minimapCanvas.SetActive(false);

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

        StartCoroutine(ShowCollectedXP());
    }

    /* 
     * Shows all the xp collected from every thing
     */
    IEnumerator ShowCollectedXP()
    {
        int xp = 0;
        foreach (KeyValuePair<string, int> pair in GameManager.Instance.enemiesKilled)
        {
           xp = GameManager.Instance.CalculateXPForEnemy(pair.Key);
            GameManager.Instance.CurrentXP += xp;
            m_TotalXPEarned += xp;

            // normalizes the currentXp to be in range of 0 - 1
            barSlider.value = (float)GameManager.Instance.CurrentXP / GameManager.Instance.maxXPAmount;

            // Adds a new textholder to the scroll view, ands sets its appopriate names and kill amounts
            EnemyTextHolder enemyTextHolder = AddEnemyTextHolder();
            enemyTextHolder.enemyNameText.text = pair.Key;
            enemyTextHolder.enemyKillText.text = "x" + pair.Value;

            // Updates total xp earned text
            totalXPText.text = "Total XP: " + m_TotalXPEarned;

            yield return new WaitForSeconds(1f);
        }

        // resets enemy kill counts
        GameManager.Instance.enemiesKilled.Clear();

        if (GameManager.Instance.IntelCollected > 0)
        {
            xp = GameManager.Instance.IntelCollected * 20;
            GameManager.Instance.CurrentXP += xp;
            m_TotalXPEarned += xp;

            barSlider.value = (float)GameManager.Instance.CurrentXP / GameManager.Instance.maxXPAmount;

            EnemyTextHolder enemyTextHolder = AddEnemyTextHolder();
            enemyTextHolder.enemyNameText.text = "Intels";
            enemyTextHolder.enemyKillText.text = "x" + GameManager.Instance.IntelCollected;

            // resets player intel collected count
            GameManager.Instance.IntelCollected = 0;

            // Updates total xp earned text
            totalXPText.text = "Total XP: " + m_TotalXPEarned;

            yield return new WaitForSeconds(1f);
        }
    }

    EnemyTextHolder AddEnemyTextHolder()
    {
        GameObject enemytextHolder = Instantiate(enemyUIPrefab, contentHolder.transform);

        // Moves all the previously added text holders down so the newest one is on top of the rest
        for (int i = 0; i < m_EnemyTextHolders.Count; i++)
        {
            RectTransform transform = m_EnemyTextHolders[i].GetComponent<RectTransform>();

            Vector3 position = transform.position;
            position.y += transform.rect.height * -1 - 10f;

            transform.position = position;
        }

        m_EnemyTextHolders.Add(enemytextHolder);

        return enemytextHolder.GetComponent<EnemyTextHolder>();
    }

    public override void Hide()
    {
        base.Hide();

        // lock mouse again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // clear all holders from scroll view 
        m_EnemyTextHolders.Clear();
    }

    public void OnNextButtonClick()
    {
        // hides this screen
        ScreenManager.Instance.HideScreen(screenName);

        // shows shop
        GameManager.Instance.shopCanvas.SetActive(true);
    }
}
