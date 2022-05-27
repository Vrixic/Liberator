using UnityEngine;

public class AmmoPickup : IPickable
{
    /* The type of ammo being picked up */
    [SerializeField] AmmoType ammoType;

    /* Amount of ammo given upon pickup */
    [SerializeField] int ammoAmount;

    public override void OnPickup(GameObject picker)
    {
        base.OnPickup(picker);

        gameObject.SetActive(false);

        AmmoManager.Instance.RefillAmmo(AmmoType.Small);
        AmmoManager.Instance.RefillAmmo(AmmoType.Shells);
    }
}
