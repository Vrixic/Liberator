using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntelPickup : IPickable
{
    public override void OnPickup(GameObject picker)
    {
        base.OnPickup(picker);
        gameObject.SetActive(false);
        GameManager.Instance.StartDisplayCashCoroutine();
    }
}
