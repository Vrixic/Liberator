using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class XPRewardScreen : BaseScreen
{
    [SerializeField] private Image rewardImage;
    [SerializeField] private TextMeshProUGUI rewardAmountText;

    [SerializeField] private List<XPReward> rewards;
    private Dictionary<int, Sprite> m_RewardSprites = new Dictionary<int, Sprite>();

    public override void Start()
    {
        base.Start();

        for (int i = 0; i < rewards.Count; i++)
            m_RewardSprites.Add(rewards[i].ID, rewards[i].rewardSprite);
    }

    public override void Show()
    {
        base.Show();

        rewardImage.sprite = m_RewardSprites[GameManager.Instance.RewardID];
        rewardAmountText.text = "x" + GameManager.Instance.RewardAmount;

        GameManager.Instance.RewardCollected = false;
    }

    public void OnCollectButtonClick()
    {
        GameManager.Instance.RewardCollected = true;
        ScreenManager.Instance.HideScreen(screenName);

        RewardPlayer();

        GameManager.Instance.OnRewardCollected?.Invoke();
    }

    private void RewardPlayer()
    {
        switch(GameManager.Instance.RewardID)
        {
            // Coins
            case 0:
                GameManager.Instance.CurrentCash += GameManager.Instance.RewardAmount;
                break;

        }
    }

    [System.Serializable]
    private class XPReward
    {
        public int ID;
        public Sprite rewardSprite;
    }
}
