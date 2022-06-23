using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardScreen : BaseScreen
{
    private const int TIME_LEADERBOARD_ID = 3838;
    private const int HEADSHOT_LEADERBOARD_ID = 3933;

    private const int MAX_SCORES = 5;

    //[SerializeField] private TMP_InputField memberID;
    [SerializeField] private Button submitButton;
    [SerializeField] private TextMeshProUGUI mainMenuButtonText;
    [SerializeField] private TextMeshProUGUI[] textHolders;
    [SerializeField] private TextMeshProUGUI[] timeHolders;
    [SerializeField] private TextMeshProUGUI[] headshotHolders;

    private Dictionary<string, int> mHeadshotPercentages = new Dictionary<string, int>();

    private bool bHasScoreBeenSubmitted = false;

    public override void Start()
    {
        base.Start();
        InitLeaderboard();
    }

    public override void Show()
    {
        base.Show();

        // unlock mouse 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (SceneManager.GetActiveScene().name == "MainMenuScene")
        {
            submitButton.gameObject.SetActive(false);
            mainMenuButtonText.text = "Close";
        }
        else
        {
            submitButton.gameObject.SetActive(true);
            mainMenuButtonText.text = "Main Menu";

            GameManager.Instance.IsUIOverlayVisible = true;
            GameManager.Instance.isXPScreenActive = true;
            GameManager.Instance.canOpenPauseMenu = false;
        }

        RefreshScores();
    }

    public void OnSubmitScoreButtonClick()
    {
        if (bHasScoreBeenSubmitted) return;

        bHasScoreBeenSubmitted = true;
        SubmitScore(TimerUI.Instance.GetElapsedTime());
    }

    public void OnMainMenuButtonClick()
    {
        if (SceneManager.GetActiveScene().name == "MainMenuScene")
        {
            ScreenManager.Instance.HideScreen(screenName);
        }
        else
        {
            PlayerPrefManager.Instance.SceneOperation = SceneManager.LoadSceneAsync("MainMenuScene");
            PlayerPrefManager.Instance.SceneOperation.allowSceneActivation = false;
            ScreenManager.Instance.ShowScreen("Transition_Screen");
        }
    }

    private void InitLeaderboard()
    {
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("Leaderboard response success!");
            }
            else
            {
                Debug.LogWarning("Leaderboard response failed: " + response.Error);
            }
        });
    }

    private void RefreshScores()
    {
        LootLockerSDKManager.GetScoreList(HEADSHOT_LEADERBOARD_ID, MAX_SCORES, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Leaderboard getting headshot scores response success!");

                LootLockerLeaderboardMember[] scores = response.items;

                int index = 0;

                for (; index < scores.Length; index++)
                {
                    Debug.Log(scores[index].member_id + " " + scores[index].score);
                    mHeadshotPercentages.Add(scores[index].member_id, scores[index].score);
                }

                GetTimeLeaderboardScores();
            }
            else
            {
                Debug.LogWarning("Leaderboard getting scores response failed!");
            }
        });

        
    }

    private void GetTimeLeaderboardScores()
    {
        LootLockerSDKManager.GetScoreList(TIME_LEADERBOARD_ID, MAX_SCORES, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Leaderboard getting time scores response success!");

                LootLockerLeaderboardMember[] scores = response.items;

                int index = 0;

                for (; index < scores.Length; index++)
                {

                    // Time and rank
                    int seconds = scores[index].score % 60;
                    int minutes = (int)(scores[index].score / 60f);
                    string text = scores[index].rank + "." + scores[index].member_id;

                    timeHolders[index].text = minutes.ToString() + (seconds < 10 ? ":0" : ":").ToString() + seconds.ToString();
                    textHolders[index].text = text;

                    // Headshot
                    headshotHolders[index].text = mHeadshotPercentages[scores[index].member_id].ToString();
                }

                if (index < MAX_SCORES)
                {
                    Debug.Log("No more entries left!");

                    for (; index < MAX_SCORES; index++)
                    {
                        Debug.Log("Filling Score[" + index + "]: " + "empty");
                        timeHolders[index].text = "";
                        textHolders[index].text = "";
                        headshotHolders[index].text = "";
                    }
                }
            }
            else
            {
                Debug.LogWarning("Leaderboard getting scores response failed!");
            }
        });
    }

    private void SubmitScore(int score)
    {
        string memberID = PlayerPrefManager.Instance.PlayerName;

        LootLockerSDKManager.SubmitScore(memberID, score, TIME_LEADERBOARD_ID, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Time Leaderboard score submitting response success!");

            }
            else
            {
                Debug.LogWarning("Time Leaderboard score submitting response failed!");
                bHasScoreBeenSubmitted = false;
            }
        });

        LootLockerSDKManager.SubmitScore(memberID, GameManager.Instance.HeadshotPercentage, HEADSHOT_LEADERBOARD_ID, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Headshot Leaderboard score submitting response success!");
            }
            else
            {
                Debug.LogWarning("Headshot Leaderboard score submitting response failed!");
                bHasScoreBeenSubmitted = false;
            }
        });

        RefreshScores();
    }
}
