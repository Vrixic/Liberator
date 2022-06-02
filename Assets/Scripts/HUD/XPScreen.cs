using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPScreen : BaseScreen
{
    [SerializeField] private Slider barSlider;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI totalXPText;

    [SerializeField] private GameObject enemyUIPrefab;
    [SerializeField] private GameObject contentHolder;

    private List<GameObject> m_EnemyTextHolders = new List<GameObject>();

    private int m_TotalXPEarned = 0;

    public override void Show()
    {
        base.Show();

        m_TotalXPEarned = 0;
        totalXPText.gameObject.SetActive(false);

        GameManager.Instance.IsXPScreenShowing = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (GameManager.Instance.GameWon)
        {
            headerText.text = "Success";
        }
        else
        {
            headerText.text = "You Died";
        }

        barSlider.value = GameManager.Instance.PreviousXP / GameManager.Instance.maxXPAmount;

        StartCoroutine(ShowCollectedXP());
    }

    IEnumerator ShowCollectedXP()
    {
        foreach (KeyValuePair<string, int> pair in GameManager.Instance.enemiesKilled)
        {
            int xp = GameManager.Instance.CalculateXPForEnemy(pair.Key);
            GameManager.Instance.CurrentXP += xp;
            m_TotalXPEarned += xp;

            barSlider.value = (float)GameManager.Instance.CurrentXP / GameManager.Instance.maxXPAmount;

            EnemyTextHolder enemyTextHolder = AddEnemyTextHolder();
            enemyTextHolder.enemyNameText.text = pair.Key;
            enemyTextHolder.enemyKillText.text = "x" + pair.Value;

            yield return new WaitForSeconds(1f);
        }

        totalXPText.gameObject.SetActive(true);
        totalXPText.text = "Total XP: " + m_TotalXPEarned;
    }

    EnemyTextHolder AddEnemyTextHolder()
    {
        GameObject enemytextHolder = Instantiate(enemyUIPrefab, contentHolder.transform);

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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_EnemyTextHolders.Clear();
    }
}
