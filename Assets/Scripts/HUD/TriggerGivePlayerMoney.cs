using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGivePlayerMoney : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CurrentCash += 10000;
        }
    }
}
