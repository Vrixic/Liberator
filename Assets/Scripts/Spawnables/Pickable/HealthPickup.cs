using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : IPickable
{
    [SerializeField] int restoreAmount;
    public override void OnDrop()
    {
        
    }
    public override void OnPickup(GameObject picker)
    {
        base.OnPickup(picker);

        Debug.Log(picker.tag + " picked up");
        if (picker.CompareTag("Player"))
        {
            GameManager.Instance.playerScript.IncreasePlayerHealth(restoreAmount);
        }
    }

    public override void Spawn()
    {
        base.Spawn();
        bJustDropped = false;
    }

    public override void Despawn()
    {
        gameObject.SetActive(false);
    }

    public override void Respawn()
    {
        gameObject.SetActive(true);
        base.Spawn();
    }
}
