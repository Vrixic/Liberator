using UnityEngine;

public class Knife : BaseMelee
{
    [SerializeField] float knifeHitRange = 0.75f;

    /*
    * triggers first attack 
    */
    public override void StartAttacking()
    {
        if (!bIsAttacking)
        {
            PlayAudioOneShot("MeleeAttack");
            GetAnimator().Play("Attack1");
            UpdateLastAttackTime();
        }
    }

    public override void StopAttacking()
    {
        base.StopAttacking();
    }

    /*
    * triggers second attack 
    */
    public override void StartAiming()
    {
        if (!bIsAttacking)
        {
            PlayAttack2Audio();
            GetAnimator().Play("Attack2");
            UpdateLastAttackTime();
        }
    }

    public override void StopAiming()
    {
        base.StopAiming();
    }

    /*
    * called when the attack animation reaches a certain frame
    */
    public override void OnAnimationEvent_AttackHit()
    {
        RaycastHit hitInfo;
        //Debug.DrawLine(raycastOrigin.position, raycastOrigin.position + GameManager.Instance.mainCamera.transform.forward * knifeHitRange, Color.red, 2f);
        if (Physics.Raycast(raycastOrigin.position, GameManager.Instance.mainCamera.transform.forward, out hitInfo, knifeHitRange, raycastLayers))
        {
            //Debug.Log(hitInfo.collider.tag);
            if (hitInfo.collider.CompareTag("Hitbox"))
            {
                if (hitInfo.collider.GetComponent<CapsuleCollider>() != null)
                {
                    //Debug.Log("Body Shot");
                    hitInfo.collider.GetComponent<Health>().TakeDamage(100.0f, transform.forward);
                }
                else
                {
                    //Debug.Log("Head Shot");
                    hitInfo.collider.GetComponentInParent<Health>().TakeDamage(100.0f, transform.forward);
                }
            }

            MeleeImpactManager.Instance.SpawnMeleeImpact(hitInfo.point, hitInfo.normal, hitInfo.collider.tag);

            // Audio
            MeleeImpactManager.Instance.PlayAudioAtLocation(transform.position, hitInfo.collider.tag);
        }
    }

    /*
     * returns if player can switch to another wepaon
    */
    public override bool CanSwitchWeapon()
    {
        return !IsAttacking();
    }
}
