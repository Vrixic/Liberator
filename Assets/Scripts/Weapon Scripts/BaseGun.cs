using Cinemachine;
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

    /* type of ammo this gun uses */
    [SerializeField] AmmoType ammoType;

    /* max number of bullets this gun can have at one time */
    [SerializeField] int maxNumOfBullets = 30;

    [SerializeField] AudioClip bulletDropAudioClip;

    /* current number of bullets */
    int m_CurrentNumOfBullets;
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

    public BaseGun()
    {

    }

    /*
    * Creates the bullet pool 
    */
    public override void Start()
    {
        base.Start();

        m_BulletPool = ObjectPoolManager.CreateObjectPool(bulletPrefab, maxNumOfBullets);
    }

    public override void Spawn()
    {
        base.Spawn();

        m_CurrentNumOfBullets = maxNumOfBullets;
        UpdateAmmoGUI();
    }

    public override void Respawn()
    {
        Spawn();
    }

    public override void OnWeaponEquip()
    {
        AmmoManager.Instance.ShowAmmoGUI();
        UpdateAmmoGUI();
    }

    public override void OnWeaponUnequip()
    {

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
        if (!HasMoreAmmo()) return; // need to reload gun, gun doesn't have enough ammo

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
        m_CurrentNumOfBullets--;

        PlayAttackAudio();

        muzzleFlash.transform.Rotate(new Vector3(0, 0, Random.Range(0f, 360f)));
        muzzleFlash.SetActive(true);

        m_MuzzleFlashTime = Time.time + muzzleFlashTime;

        if (bIsAiming)
            GetAnimator().Play("Aim_Attack1");
        else
            GetAnimator().Play("Attack1");

        ShootBullet();

        PlayBulletDropAudio();
        UpdateAmmoGUI();
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
            PlayerLook.pendingYRecoil = verticalRecoil[m_CurrentRecoilIndex]; 
            PlayerLook.pendingXRecoil = horizontalRecoil[m_CurrentRecoilIndex];
        }


        StartShooting();
    }

    /*
     * Spawns bullet
     */
    private void ShootBullet()
    {
        Bullet bullet = ObjectPoolManager.SpawnObject(m_BulletPool) as Bullet;

        RaycastHit hitInfo;
        if (Physics.Raycast(raycastOrigin.position, GameManager.Instance.mainCamera.transform.forward, out hitInfo, bulletRange, raycastLayers))
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
    void OnRayCastHit(Bullet bullet, RaycastHit hit)
    {
        //mDebug.Log(hit.collider.tag);
        if (hit.collider.CompareTag("Hitbox"))
        {
            //Debug.LogWarning("enemys cannot be hurt as of right now, updated bullet script");
            hit.collider.GetComponent<Health>().TakeDamage(GetDamage(), transform.forward);
        }

        bullet.Spawn(raycastOrigin.position, GameManager.Instance.mainCamera.transform.forward, hit, 0.5f);
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
        if (!HasMoreAmmoInPouch()) return; // no more ammo left 

        StopAiming();
        StartWeaponReloading();
    }

    /*
    * Start Weapon Reloading
    */
    public void StartWeaponReloading()
    {
        bIsReloading = true;
        GetAnimator().SetTrigger("Reload");
    }

    public override void OnAnimationEvent_ReloadStart()
    {
        m_CurrentNumOfBullets += AmmoManager.Instance.GetAmmo(ammoType, maxNumOfBullets - m_CurrentNumOfBullets);
        PlayRelaodAudio();

        //Debug.Log(name + ": Reloading started");
    }

    public override void OnAnimationEvent_ReloadEnd()
    {
        //Debug.Log(name + ": Reloading ended");

        UpdateAmmoGUI();
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
        PlayAudioOneShot(bulletDropAudioClip);
    }

    /*
    * plays the muzzle flash animation
    */
    void HideMuzzleFlash()
    {
        muzzleFlash.SetActive(false);
    }

    void PlayRelaodAudio()
    {
        PlayAudioOneShot(reloadAudioClip);
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
}
