using UnityEngine.UI;
using LootLocker.Requests;
using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    private static LeaderboardController mInstance;
    public static LeaderboardController Instance { get { return mInstance; } }

    [SerializeField] private InputField memberID;

    private const int ID = 3838;
    private const int MAX_SCORES = 5;

    private void Awake()
    {
        if (mInstance == null)
            mInstance = this;
    }

    private void Start()
    {
        LootLockerSDKManager.StartSession("Player", (response) =>
        {
            if (response.success)
            {
                Debug.Log("Leaderboard response success!");
            }
            else
            {
                Debug.LogWarning("Leaderboard response failed!");
            }
        });
    }

    public void SubmitScore(int score)
    {
        if (memberID.text.Length < 6)
        {
            Debug.Log("Leaderboard score submitting failed: member id < 6 characters");
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
            }
        });
    }

    public void ShowScores()
    {
        LootLockerSDKManager.GetScoreList(ID, MAX_SCORES, (response) => {
            if (response.success)
            {
                Debug.Log("Leaderboard getting scores response success!");

                LootLockerLeaderboardMember[] scores = response.items;

                int length = Mathf.Clamp(0, MAX_SCORES, scores.Length);
                int index = 0;

                for (; index < length; index++)
                {
                    Debug.Log("Rank: " + scores[index].rank + ", Scores[" + index + "]: " + scores[index].score);
                }

                if (index < MAX_SCORES)
                {
                    Debug.Log("No more entries left!");

                    for(; index < MAX_SCORES; index++)
                    {
                        Debug.Log("Filling Score[" + index + "]: " + "empty");
                    }
                }
            }
            else
            {
                Debug.LogWarning("Leaderboard getting scores response failed!");
            }
        });
    }
}
