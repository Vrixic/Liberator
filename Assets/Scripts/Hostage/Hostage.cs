using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hostage : MonoBehaviour
{
    [SerializeField] public HostageDoorController doorToOpenWhenHostageSecured = null;
    [SerializeField] private GameObject levelToLoad = null;
    [SerializeField] private GameObject levelToUnload = null;

    private int health = 100;

    public static int hostagesSecured = 0;

    public void HostageSecured()
    {
        hostagesSecured++;

        if(levelToLoad != null)
            levelToLoad.SetActive(true);
        
        if (levelToUnload != null)
            levelToUnload.SetActive(false);

        if(doorToOpenWhenHostageSecured != null)
        {
            doorToOpenWhenHostageSecured.OpenHostageDoor();
        }

        gameObject.SetActive(false);
    }

    public void TakeDamage(int d)
    {
        health -= d;

        if (health < 0) // Player lost the game
        {
            EnemyHitFeedbackManager.Instance.ShowHitFeedback(Color.red);

            GameManager.Instance.GameWon = false;
            GameManager.Instance.HostageDied = false;

            Invoke("ResetGame", 0.3f);

            return;
        }

        EnemyHitFeedbackManager.Instance.ShowHitFeedback(Color.white);
    }

    public void ResetGame()
    {
        GameManager.Instance.ResetGame();
    }
}
