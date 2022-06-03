using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : IPickable
{
    [SerializeField] int restoreAmount;

    public override void OnPickup(GameObject picker)
    {
        base.OnPickup(picker);

        PlayPickupAudio("HealthKit");

        gameObject.SetActive(false);
        GameManager.Instance.playerScript.IncreasePlayerHealth(restoreAmount);
    }
}
