using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : IPickable
{
    [SerializeField] int restoreAmount;

    public override void OnPickup(GameObject picker)
    {
        base.OnPickup(picker);

        gameObject.SetActive(false);
        GameManager.Instance.playerScript.IncreasePlayerHealth(GameManager.Instance.playerScript.GetPlayersMaxHealth());
        GameManager.Instance.StartDisplayHealthGainedCoroutine();
    }
}
