using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardScreen : BaseScreen
{
    private const int ID = 3838;
    private const int MAX_SCORES = 5;    

    [SerializeField] private TMP_InputField memberID;
    [SerializeField] private TextMeshProUGUI[] textHolders;

    private bool bHasScoreBeenSubmitted = false;

    public override void Start()
    {
        base.Start();
        InitLeaderboard();
    }

    public override void Show()
    {
        base.Show();

        GameManager.Instance.IsUIOverlayVisible = true;
        GameManager.Instance.isXPScreenActive = true;
        GameManager.Instance.canOpenPauseMenu = false;

        // unlock mouse 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowScores();
    }

    public void OnSubmitScoreButtonClick()
    {
        if (bHasScoreBeenSubmitted) return;

        SubmitScore(TimerUI.Instance.GetElapsedTime());
        bHasScoreBeenSubmitted = true;
        ShowScores();
    }

    public void OnMainMenuButtonClick()
    {
        PlayerPrefManager.Instance.SceneOperation = SceneManager.LoadSceneAsync(0);
        PlayerPrefManager.Instance.SceneOperation.allowSceneActivation = false;
        ScreenManager.Instance.ShowScreen("Transition_Screen");
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

    private void ShowScores()
    {
        LootLockerSDKManager.GetScoreList(ID, MAX_SCORES, (response) => {
            if (response.success)
            {
                Debug.Log("Leaderboard getting scores response success!");

                LootLockerLeaderboardMember[] scores = response.items;

                int index = 0;

                for (; index < scores.Length; index++)
                {
                    Debug.Log("Rank: " + scores[index].rank + ", Scores[" + index + "]: " + scores[index].score);
                    int seconds = scores[index].score % 60;
                    int minutes = (int)(scores[index].score / 60f);
                    string text = scores[index].rank + ".   " + scores[index].member_id + "   " + minutes.ToString() + (seconds < 10 ? ":0" : ":") + seconds.ToString();
                    textHolders[index].text =  text;
                }

                if (index < MAX_SCORES)
                {
                    Debug.Log("No more entries left!");

                    for (; index < MAX_SCORES; index++)
                    {
                        Debug.Log("Filling Score[" + index + "]: " + "empty");
                        string text = index + ".   " + "empty";
                        textHolders[index].text = text;
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
        if (memberID.text.Length < 6 || memberID.text.Length > 12)
        {
            Debug.Log("Leaderboard score submitting failed: member id < 6 characters or > 12 characters");
            bHasScoreBeenSubmitted = false;
            return;
        }

        LootLockerSDKManager.SubmitScore(memberID.text, score, ID, (response) => {
            if (response.success)
            {
                Debug.Log("Leaderboard score submitting response success!");
            }
            else
            {
                Debug.LogWarning("Leaderboard score submitting response failed!");
                bHasScoreBeenSubmitted = false;
            }
        });
    }
}
