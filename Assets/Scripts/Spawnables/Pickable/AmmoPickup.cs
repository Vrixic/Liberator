using UnityEngine;

public class AmmoPickup : IPickable
{
    /* The type of ammo being picked up */
    [SerializeField] AmmoType ammoType;

    /* Amount of ammo given upon pickup */
    [SerializeField] int ammoAmount;
    public override void OnDrop()
    {

    }

    public override void OnPickup(GameObject picker)
    {
        base.OnPickup(picker);
        if (picker.tag == "Player")
        {
            //AmmoManager.Instance.IncreaseAmmo(ammoType, ammoAmount
            AmmoManager.Instance.RefillAmmo(AmmoType.Small);
            AmmoManager.Instance.RefillAmmo(AmmoType.Shells);
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
