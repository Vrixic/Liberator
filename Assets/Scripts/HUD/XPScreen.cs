using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class XPScreen : BaseScreen
{
    /* XP bar slider */
    [SerializeField] private Slider barSlider;

    /* Display text for if player won or loss the game */
    [SerializeField] private TextMeshProUGUI headerText;

    /*  */
    [SerializeField] private TextMeshProUGUI buttonText;

    /* Displays the total xp earned from this run */
    [SerializeField] private TextMeshProUGUI totalXPText;

    [SerializeField] private TextMeshProUGUI totalSkillPointsText;

    /* prefab of enemy text holder */
    [SerializeField] private GameObject enemyUIPrefab;

    /* where all the enemy text holders will go under, their parent */
    [SerializeField] private GameObject contentHolder;

    [SerializeField] [Tooltip("Time it takes to fill the bar up from previous to current total")] private float timeBetweenUIBlocks = 1f;

    /* tracks all the enemy text holders created */
    private List<EnemyTextBlock> m_EnemyTextHolders = new List<EnemyTextBlock>();

    /* Bar fill and Ui Block animation speed */
    private float m_UIBlocknAnimationSpeed = 0f;

    private float m_BarAnimationSpeed = 0f;

    /* Previous amount of xp */
    private float m_PreviousXP = 0f;

    private float m_CurrentXP = 0f;

    private bool m_AnimationFinished = false;

    //private bool bRewardScreenShowing = false;
    private bool bStopAddingBlocks = false;

    public override void Start()
    {
        base.Start();

        m_UIBlocknAnimationSpeed = 15 / timeBetweenUIBlocks;

        //GameManager.Instance.OnRewardCollected += OnRewardCollected;
    }

    public override void Show()
    {
        base.Show();

        // Disables minimap canvas
        GameManager.Instance.minimapCanvas.SetActive(false);

        m_CurrentXP = PlayerPrefManager.Instance.CurrentXP;
        m_PreviousXP = PlayerPrefManager.Instance.CurrentXP;

        GameManager.Instance.IsUIOverlayVisible = true;

        // unlock mouse 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Update header text
        if (GameManager.Instance.GameWon)
        {
            headerText.text = "Success";
            buttonText.text = "Next";
        }
        else
        {
            buttonText.text = "Main Menu";
            headerText.text = "";
        }

        // set the bars value to the previous xp earned from last run
        barSlider.value = m_PreviousXP / GameManager.Instance.maxXPAmount;

        m_AnimationFinished = false;

        //bRewardScreenShowing = false;
        bStopAddingBlocks = false;

        totalSkillPointsText.text = "Skill Points: " + PlayerPrefManager.Instance.currentSkillPoints;

        StartCoroutine(ShowCollectedXP());
    }

    public override void Update()
    {
        if (m_AnimationFinished) return; //|| bRewardScreenShowing) return;

        // Moves all of the UI Blocks down 
        for (int i = 0; i < m_EnemyTextHolders.Count; i++)
        {
            RectTransform trans = m_EnemyTextHolders[i].holder.GetComponent<RectTransform>();

            Vector2 targetPos = trans.anchoredPosition;
            int multiplier = m_EnemyTextHolders.Count - i;
            targetPos.y = trans.rect.height * multiplier * -1 + 45f - (multiplier * 10f);

            trans.anchoredPosition = Vector2.Lerp(trans.anchoredPosition, targetPos, m_UIBlocknAnimationSpeed * Time.unscaledDeltaTime);
        }

        //Debug.Log("Animating....");
        m_CurrentXP = Mathf.Lerp(m_CurrentXP, PlayerPrefManager.Instance.CurrentXP, m_BarAnimationSpeed * Time.unscaledDeltaTime);
        barSlider.value = m_CurrentXP / GameManager.Instance.maxXPAmount;

        totalXPText.text = "XP: " + ((int)m_CurrentXP).ToString() + "/" + GameManager.Instance.maxXPAmount.ToString();

        if (PlayerPrefManager.Instance.CurrentXP >= 100f)
        {
            bStopAddingBlocks = true;

            if (m_CurrentXP >= 99f)
            {
                //bRewardScreenShowing = true;
                GiveSkillPoint();
            }
        }
        else
        {
            bStopAddingBlocks = false;
        }
    }

    public override void Hide()
    {
        base.Hide();

        // lock mouse again
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        // clear all holders from scroll view 
        for (int i = 0; i < m_EnemyTextHolders.Count; i++)
            Destroy(m_EnemyTextHolders[i].holder.gameObject);

        m_EnemyTextHolders.Clear();

        // resets enemy kill counts
        GameManager.Instance.enemiesKilled.Clear();
    }

    public void OnNextButtonClick()
    {
        if (!m_AnimationFinished) return;
        // hides this screen
        ScreenManager.Instance.HideScreen(screenName);
        ScreenManager.Instance.HideScreen("Death_Screen");

        PlayerPrefManager.Instance.SaveGame();

        if (GameManager.Instance.GameWon)
        {
            // shows shop
            //GameManager.Instance.buttonFuncScript.OpenShopMenu();

            AudioManager.Instance.PlayAudioAtLocation(GameManager.Instance.currentHostageDoorTransform.position, "DoorOpen");

            GameManager.Instance.minimapCanvas.SetActive(true);

            GameManager.Instance.IsUIOverlayVisible = false;

            // unlock mouse 
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Time.timeScale = 1f;
        }
        else
        {
            PlayerPrefManager.Instance.SceneOperation = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
            PlayerPrefManager.Instance.SceneOperation.allowSceneActivation = false;
            ScreenManager.Instance.ShowScreen("Transition_Screen");
        }
    }

    /* 
     * Shows all the xp collected from every thing
     */
    IEnumerator ShowCollectedXP()
    {
        foreach (KeyValuePair<string, int> pair in GameManager.Instance.enemiesKilled)
        {
            while (bStopAddingBlocks)
            {
                yield return null;
            }

            PlayerPrefManager.Instance.CurrentXP += GameManager.Instance.CalculateXPForEnemy(pair.Key);

            EnemyTextBlock block = new EnemyTextBlock();
            // Adds a new textholder to the scroll view, ands sets its appopriate names and kill amounts            
            block.holder = AddEnemyTextHolder();
            switch (pair.Key)
            {
                case "BasicEnemy":
                    block.holder.enemyNameText.text = "Soldier";
                    break;
                case "MeleeUnit":
                    block.holder.enemyNameText.text = "Stabber";
                    break;
                default:
                    block.holder.enemyNameText.text = pair.Key;
                    break;
            }
            
            block.holder.enemyKillText.text = "x" + pair.Value;

            block.transform = block.holder.GetComponent<RectTransform>();

            m_EnemyTextHolders.Add(block);

            UpdateTotalXPText();

            CalculateBarSpeed();

            yield return new WaitForSecondsRealtime(timeBetweenUIBlocks);
        }

        if (GameManager.Instance.IntelCollected > 0)
        {
            while (bStopAddingBlocks)
            {
                yield return null;

            }

            PlayerPrefManager.Instance.CurrentXP += GameManager.Instance.IntelCollected * 20;

            //barSlider.value = (float)GameManager.Instance.CurrentXP / GameManager.Instance.maxXPAmount;

            EnemyTextBlock block = new EnemyTextBlock();
            // Adds a new textholder to the scroll view, ands sets its appopriate names and kill amounts            
            block.holder = AddEnemyTextHolder();
            block.holder.enemyNameText.text = "Intels";
            block.holder.enemyKillText.text = "x" + GameManager.Instance.IntelCollected;

            block.transform = block.holder.GetComponent<RectTransform>();

            m_EnemyTextHolders.Add(block);

            // resets player intel collected count
            GameManager.Instance.IntelCollected = 0;

            UpdateTotalXPText();

            CalculateBarSpeed();

            yield return new WaitForSecondsRealtime(timeBetweenUIBlocks);
        }

        while (PlayerPrefManager.Instance.CurrentXP >= 100f)
        {
            GiveSkillPoint();
            yield return new WaitForSecondsRealtime(0.1f);
        }

        yield return new WaitForSecondsRealtime(timeBetweenUIBlocks);

        m_AnimationFinished = true;
    }

    private void UpdateTotalXPText()
    {
        // Updates total xp earned text
        totalXPText.text = "Total XP: " + PlayerPrefManager.Instance.CurrentXP;
    }

    void GiveSkillPoint()
    {
        PlayerPrefManager.Instance.currentSkillPoints++;
        PlayerPrefManager.Instance.CurrentXP -= 100;
        m_PreviousXP = 0;
        m_CurrentXP = 0;

        CalculateBarSpeed();
        UpdateTotalXPText();

        totalSkillPointsText.text = "Skill Points: " + PlayerPrefManager.Instance.currentSkillPoints;
    }

    //void ShowRewardScreen()
    //{
    //    //GameManager.Instance.RewardID = 0;
    //    //GameManager.Instance.RewardAmount = 100;
    //    //ScreenManager.Instance.ShowScreen("XP_Reward_Screen");

    //    PlayerPrefManager.Instance.currentSkillPoints++;
    //    PlayerPrefManager.Instance.CurrentXP -= 100;
    //    m_PreviousXP = 0;
    //    m_CurrentXP = 0;

    //    CalculateBarSpeed();
    //    UpdateTotalXPText();
    //}

    //void OnRewardCollected()
    //{
    //    bRewardScreenShowing = false;
    //}

    private void CalculateBarSpeed()
    {
        m_BarAnimationSpeed = (Mathf.Clamp(PlayerPrefManager.Instance.CurrentXP, 0, 100) - m_PreviousXP) / timeBetweenUIBlocks * 0.5f;
    }

    EnemyTextHolder AddEnemyTextHolder()
    {
        GameObject enemytextHolder = Instantiate(enemyUIPrefab, contentHolder.transform);
        return enemytextHolder.GetComponent<EnemyTextHolder>();
    }

    private class EnemyTextBlock
    {
        public EnemyTextHolder holder;
        public RectTransform transform;
    }
}
