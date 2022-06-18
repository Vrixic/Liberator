using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseGun : BaseWeapon
{
    [SerializeField] AudioClip reloadAudioClip;

    //[SerializeField] Transform gunSpawnLocation;

    [Header("Visuals")]

    /* Muzzle flash VFX played when weapon has been fired */
    [SerializeField] GameObject muzzleFlash;

    /* time muzzle flash stays for */
    [SerializeField] float muzzleFlashTime = 0.05f;

    [Header("Bullet")]

    #region Bullet Stuff
    /* prefab of the bullet that will be spawned in */
    [SerializeField] Bullet bulletPrefab;

    /* the max range a bullet can travel before getting destroyed */
    [SerializeField] [Tooltip("Squared range")] float bulletRange = 10000f;

    [SerializeField] [Tooltip("Damage fall off percentage per meter traveled")] [Range(0f, 1f)] float damageFallOffPercentage = 0.15f;

    [SerializeField] [Tooltip("Any damage fall off percentage lower than this will be discarded")] [Range(0f, 0.5f)] float minDamageFallOff = 0.5f;

    /* type of ammo this gun uses */
    [SerializeField] AmmoType ammoType;

    /* max number of bullets this gun can have at one time */
    [SerializeField] int maxNumOfBullets = 30;

    [SerializeField] AudioClip bulletDropAudioClip;

    /* current number of bullets */
    protected int m_CurrentNumOfBullets;
    #endregion

    [Header("Recoil")]
    [SerializeField] List<float> verticalRecoil;
    [SerializeField] List<float> horizontalRecoil;

    int m_CurrentRecoilIndex = 0;

    /* bool to keep in track if weapon is being reloaded */
    protected bool bIsReloading = false;

    /* bool to keep in track if weapon is aimed or not */
    protected bool bIsAiming = false;

    protected bool bIsStabilized = false;

    /* time when muzzle flash was shown plus the timer */
    float m_MuzzleFlashTime = 0f;

    /* a objectpool that will keep track of spawned bullets automatically */
    string m_BulletPool;

    /*
    * Creates the bullet pool 
    */
    public override void Start()
    {
        base.Start();

        m_BulletPool = ObjectPoolManager.Instance.CreateObjectPool(bulletPrefab, maxNumOfBullets);
        damageFallOffPercentage = 1 - damageFallOffPercentage;

        m_CurrentNumOfBullets = maxNumOfBullets;
        UpdateAmmoGUI();
    }

    public override void OnWeaponEquip()
    {
        base.OnWeaponEquip();

        AmmoManager.Instance.ShowAmmoGUI();
        UpdateAmmoGUI();
    }

    public override void OnWeaponUnequip()
    {
        base.OnWeaponUnequip();
    }

    /*
    * Checks if reloading needs to be stopped, if so, it is stopped
    * 
    * If gun runs out of ammo and has more in pounch then reloads automatically
    */
    public override void Update()
    {
        if (!bIsReloading && !HasMoreAmmo() && HasMoreAmmoInPouch())
            Reload();

        if (Time.time > m_MuzzleFlashTime)
            HideMuzzleFlash();
    }

    /*
    * plays the shoot animation and shoots the bullet
    */
    public override void StartAttacking()
    {
        if (bIsReloading) return; // dont do anything when gun is being reloaded
        if (!HasMoreAmmo()) { 
            AudioManager.Instance.PlayAudioAtLocation(transform.position, "OutOfAmmo");
            return;
        }
         // need to reload gun, gun doesn't have enough ammo

        if (TakeAction(m_LastAttackTime, attackRate)) // can shoot this weapon now
        {
            if (bIsStabilized)
            {
                StartShooting();
            }
            else
            {
                ShootWithRecoil();
            }

            GameManager.Instance.AlertEnemiesInSphere(transform.position, 14);
        }
    }

    /*
    * Start Weapon Fire
    * 
    * Spawns bullet which travels in the direction of @Member Field 'fpCamera' foward vector
    * Spawn location varies on if weapon is aimed (@Member Field 'fpCamera' position vector is used), if not (@Member Field 'bulletSpawnLocation' position vector is used)
    */
    public void StartShooting()
    {
        bIsAttacking = true;

        m_LastAttackTime = Time.time;
        UseAmmo();

        PlayAttackAudio();

        muzzleFlash.transform.Rotate(new Vector3(0, 0, Random.Range(0f, 360f)));
        muzzleFlash.SetActive(true);

        m_MuzzleFlashTime = Time.time + muzzleFlashTime;

        if (bIsAiming)
            GetAnimator().Play("Aim_Attack1");
        else
            GetAnimator().Play("Attack1");

        ShootBullet();

        //apply camera shake
        if (GameManager.Instance.cameraShakeScript.Trauma < 0.10f)
            GameManager.Instance.cameraShakeScript.Trauma += 0.25f;

        PlayBulletDropAudio();
        UpdateAmmoGUI();
    }

    protected virtual void UseAmmo()
    {
        m_CurrentNumOfBullets--;
    }

    /*
     * adds recoil than shoots the bullet
     */
    public virtual void ShootWithRecoil()
    {
        //Debug.Log("BaseGun 'ShoowWithRecoil()' : doesn't need an implementation, for child classes");

        if ((m_CurrentRecoilIndex + 1) == verticalRecoil.Count)
        {
            m_CurrentRecoilIndex = 0;
        }
        else
        {
            m_CurrentRecoilIndex++;
        }

        if (verticalRecoil.Count > 0)
        {
            PlayerRecoil.incomingVerticalRecoil = verticalRecoil[m_CurrentRecoilIndex];
            PlayerRecoil.incomingHorizontalRecoil = horizontalRecoil[m_CurrentRecoilIndex];
        }

        StartShooting();
    }

    /*
     * Spawns bullet
     */
    public virtual void ShootBullet()
    {
        Bullet bullet = ObjectPoolManager.Instance.SpawnObject(m_BulletPool) as Bullet;

        RaycastHit hitInfo;
        if (Physics.Raycast(GameManager.Instance.mainCamera.transform.position, GameManager.Instance.playerAimVector, out hitInfo, bulletRange, raycastLayers))
        {
            OnRayCastHit(bullet, hitInfo);
        }
        else
        {
            bullet.Spawn(raycastOrigin.position, GameManager.Instance.mainCamera.transform.forward, bulletRange);
        }
    }

    /*
     * Bullet collided with an object
     * Spawns a impact particle based on what was hit
     * Invokes disable method for bullet to get pooled
     */
    protected void OnRayCastHit(Bullet bullet, RaycastHit hit)
    {
        Vector3 deltaPosition = hit.point - raycastOrigin.position;

        float damageFallOff = GetDamageFallOff(deltaPosition);
        //Debug.Log(damageFallOff);

        //Debug.Log(hit.collider.tag);
        Hostage hostage;
        if (hit.collider.CompareTag("Hitbox"))
        {
            if (hit.collider.GetComponent<CapsuleCollider>() != null)
            {
                //Debug.Log("Body Shot");
                hit.collider.GetComponent<Health>().TakeDamage(GetDamage() * damageFallOff, transform.forward);
            }
            else
            {
                //Debug.Log("Head Shot");
                hit.collider.GetComponentInParent<Health>().TakeDamage(100.0f, transform.forward);
            }

        }
        else if ((hostage = hit.collider.GetComponentInParent<Hostage>()) != null)
        {
            if (hit.collider.GetComponent<Headshot_Hitbox>() != null)
                hostage.TakeDamage(100);
            else
                hostage.TakeDamage(GetDamage()); 
        }

        bullet.Spawn(raycastOrigin.position, deltaPosition.normalized, hit, 0.5f);
    }

    protected float GetDamageFallOff(Vector3 hitPosition)
    {
        float distance = hitPosition.sqrMagnitude;
        float damageFallOff = ((bulletRange / distance) * damageFallOffPercentage);

        if (damageFallOff < minDamageFallOff)
            damageFallOff = minDamageFallOff;
        else if (damageFallOff > 1f)
            damageFallOff = 1f;

        return damageFallOff;
    }

    public override void StopAttacking()
    {
        bIsAttacking = false;
    }

    /*
    * plays reload animation and reloads the gun
    */
    public override void Reload()
    {
        if (bIsReloading) return; // gun is already reloading
        if (IsAtMaxAmmo()) return; // doesn't allow reload when gun has full ammo
        if (!HasMoreAmmoInPouch())
        {
            AudioManager.Instance.PlayAudioAtLocation(transform.position, "OutOfAmmo");
            return; // no more ammo left 
        }

        StopAiming();
        StartWeaponReloading();
    }

    /*
    * Start Weapon Reloading
    */
    public void StartWeaponReloading()
    {
        bIsReloading = true;
        if (GetWeaponID() == WeaponID.Shotgun)
        {
            StartCoroutine(ReloadingShotgun());
        }
        else
        {
            GetAnimator().SetTrigger("Reload");
        }
    }

    // Ienumerator for reloading shotgun 1 bullet at a time
    IEnumerator ReloadingShotgun()
    {
        GetAnimator().SetBool("isReloading", true);

        yield return new WaitForSeconds(0.3f);
        while (m_CurrentNumOfBullets < maxNumOfBullets)
        {
            if (HasMoreAmmoInPouch())
            {
                if ((m_CurrentNumOfBullets == maxNumOfBullets - 1) || AmmoManager.Instance.GetAmmoAmount(AmmoType.Shells) == 1)
                {
                    GetAnimator().Play("EndReload");

                    GetAnimator().SetBool("isReloading", false);
                }
                else
                {
                    GetAnimator().Play("ReloadBullet");
                }
            }
            else
            {
                GetAnimator().SetBool("isReloading", false);
                break;
            }

            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(0.3f);
    }

    // event that is called when the clip is taken from the weapon
    public override void OnAnimationEvent_ReloadRemoveMag()
    {
        AudioManager.Instance.PlayAudioAtLocation(transform.position, GetWeaponID().ToString() + "_ReloadStart");
    }

    // event that is called when the clip is put back into the weapon
    public override void OnAnimationEvent_ReloadReplaceMag()
    {
        AudioManager.Instance.PlayAudioAtLocation(transform.position, GetWeaponID().ToString() + "_ReloadMiddle");

        if (GetWeaponID() == WeaponID.Shotgun)
        {
            m_CurrentNumOfBullets += AmmoManager.Instance.GetAmmo(ammoType, 1);
            UpdateAmmoGUI();
        }
    }

    //event that is called at the end of the animation, for certain weapon sounds
    public override void OnAnimationEvent_ReloadEnd()
    {
        AudioManager.Instance.PlayAudioAtLocation(transform.position, GetWeaponID().ToString() + "_ReloadEnd");
        if (GetWeaponID() != WeaponID.Shotgun)
        {
            m_CurrentNumOfBullets += AmmoManager.Instance.GetAmmo(ammoType, maxNumOfBullets - m_CurrentNumOfBullets);
            UpdateAmmoGUI();
        }
        StopReloading();
    }

    /*
    * stops reloading
    */
    public void StopReloading()
    {
        bIsReloading = false;
    }

    /*
    * starts aiming weapon
    */
    public override void StartAiming()
    {
        if (!bIsReloading)
        {
            // Debug.LogWarning("uncomment start aiming animation code out");
            GetAnimator().SetBool("isAiming", true);
            bIsAiming = true;
        }
    }

    /*
    * stops weapon aiming
    */
    public override void StopAiming()
    {
        //Debug.LogWarning("uncomment stop aiming animation code out");

        if (bIsAiming)


            GetAnimator().SetBool("isAiming", false);
        bIsAiming = false;

        // transform.localPosition = defaultWeaponLocalPosition;
    }

    /*
    * returns if player can switch to another wepaon
    */
    public override bool CanSwitchWeapon()
    {
        return !bIsReloading;
    }

    void PlayBulletDropAudio()
    {
        PlayAudioOneShot("BulletDropped");
    }

    /*
    * plays the muzzle flash animation
    */
    void HideMuzzleFlash()
    {
        muzzleFlash.SetActive(false);
    }

    /*
    * returns if gun has ammo 
    */
    public bool HasMoreAmmo()
    {
        return m_CurrentNumOfBullets > 0;
    }

    /*
     * returns if num bullets are > 0
     */
    public bool HasMoreAmmoInPouch()
    {
        return AmmoManager.Instance.GetAmmoAmount(ammoType) > 0;
    }

    /*
    * returns guns ammo type
    */
    public AmmoType GetAmmoType()
    {
        return ammoType;
    }

    /*
    * returns guns current ammo
    */
    public int GetCurrentBullets()
    {
        return m_CurrentNumOfBullets;
    }

    /*
    * returns if guns ammo is at max 
    */
    public bool IsAtMaxAmmo()
    {
        return m_CurrentNumOfBullets == maxNumOfBullets;
    }


    /*
    * returns if the weapon is reloading or not
    */
    public override bool IsWeaponReloading()
    {
        return bIsReloading;
    }

    /*
    * returns if weapon is aimed or not
    */
    public override bool IsWeaponAimed()
    {
        return bIsAiming;
    }

    /*
    * Update the ammo gui text
    */
    public override void UpdateAmmoGUI()
    {
        AmmoManager.Instance.UpdateAmmoGUI(ammoType, m_CurrentNumOfBullets);
    }

    public override void SetRecoilPatternIndex(int i)
    {
        m_CurrentRecoilIndex = i;
    }

    public override int GetRecoilPatternIndex()
    {
        return m_CurrentRecoilIndex;
    }

    protected string GetBulletPool()
    {
        return m_BulletPool;
    }

    protected float GetBulletRange()
    {
        return bulletRange;
    }
}
