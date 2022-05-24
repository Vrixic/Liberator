using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : ISpawnable
{
    [Header("Weapon Settings")]

    /* array of weapons */
    [SerializeField] WeaponID[] startWeaponIDs;
    BaseWeapon[] m_CurrentWeapons;

    [SerializeField] GameObject weaponsParent;

    /* base throwable of type flashbang */
    [SerializeField] FlashbangHand flashbang;
    [SerializeField] BaseThrowableHands sensor;

    [Header("Flashbang Settings")]

    /*
   * used to slow down or speed up how long flashbang lasts
   */
    [SerializeField] float flashbangTimeDamp = 0.25f;

    /*
    * color of the flashbang -- temporary
    */
    [SerializeField] Color flashbangImageColor;

    [Header("Player Settings")]

    /* max health player can have */
    [SerializeField] int maxPlayerHealth = 100;

    /* max health player can have */
    [SerializeField] int maxPlayerShield = 100;

    /* amount of percentage of damage the shield can intake when player has been damaged */
    [SerializeField] float playerShieldFallOffScale = 0.8f;

    [Header("Animation Settings")]

    /* time it takes to lerp from last animation speed to current animation speed */
    [SerializeField] float animationSpeedInterpTime = 0.1f;

    [Header("Audio Settings")]

    [SerializeField] AudioSource weaponAudioSrc;

    /* footstep audio when player walks */
    [SerializeField] AudioClip footStepWalkAudio;

    /* footstep audio when player runs */
    [SerializeField] AudioClip footStepRunAudio;

    /* delay on the foot step walk audio to keep it in control */
    [SerializeField] float footStepWalkAudioPlayDelay = 0.75f;

    /* delay on the foot step run audio to keep it in control */
    [SerializeField] float footStepRunAudioPlayDelay = 0.75f;

    /* current equipped weapon */
    int m_CurrentWeaponIndex;

    /* current player health */
    int m_CurrentPlayerHealth;

    /* current player shield */
    int m_CurrentPlayerShield;

    /* Last Animation speed */
    float m_LastAnimSpeed = 0f;

    /* audio source used for footsteps */
    AudioSource m_FootstepAudioSrc;

    /* last time a sound effect was played for footsteps */
    float m_LastStepSoundTime = 0f;

    /* player motor of the player */
    PlayerMotor m_PlayerMotor;

    /* current equipped weapon */
    BaseWeapon m_CurrentEquippedWeapon;

    /* Health bar connected to player, in inspector must select health bar from ui to work*/
    HealthBar healthBar;

    /* Shield bar connected to player, in inspector must select Shield bar from ui to work*/
    ShieldBar shieldBar;

    bool bPlayerWantsToAttack = false;

    private void Start()
    {
        m_PlayerMotor = GetComponent<PlayerMotor>();
        m_FootstepAudioSrc = GetComponentInChildren<AudioSource>();

        healthBar = GameManager.Instance.healthBarScript;
        shieldBar = GameManager.Instance.shieldBarScript;

        m_CurrentWeapons = new BaseWeapon[startWeaponIDs.Length];

        Spawn();

        flashbang.OnPickup(weaponsParent);
        sensor.OnPickup(weaponsParent);

        DeactivateFlashbang();
        DeactivateSensor();
    }

    public override void Spawn()
    {
        for (int i = 0; i < startWeaponIDs.Length; i++)
        {
            m_CurrentWeapons[i] = WeaponSpawnManager.Instance.GetWeapon(startWeaponIDs[i], weaponsParent.transform);
            m_CurrentWeapons[i].OnPickup(weaponsParent);
        }

        m_CurrentWeaponIndex = 0;
        ActivateWeapon(0);

        //m_CurrentEquippedWeapon = m_CurrentWeapons[m_CurrentWeaponIndex];
        //m_CurrentEquippedWeapon.gameObject.SetActive(true);

        UpdateFlashbangCount();
        UpdateSensorGrenadeUi();

        m_CurrentPlayerHealth = maxPlayerHealth;
        m_CurrentPlayerShield = maxPlayerShield;

        healthBar.SetMaxHealth();
        shieldBar.SetMaxShield();

    }

    public override void Despawn()
    {
        DeactivateFlashbang();
        DeactivateWeapon(m_CurrentWeaponIndex);

        Respawn();
    }

    public override void Respawn()
    {
        base.Respawn();

        for (int i = 0; i < m_CurrentWeapons.Length; i++)
        {
            m_CurrentWeapons[i].Respawn();
            m_CurrentWeapons[i].OnPickup(weaponsParent);
        }

        Spawn();
    }

    private void Update()
    {
        if (GameManager.Instance.playerIsGrounded && m_PlayerMotor.currentActiveSpeed2D > 0.1f)
        {
            if (Time.time - m_LastStepSoundTime > footStepWalkAudioPlayDelay && m_PlayerMotor.currentActiveSpeed2D < 0.3f)
            {
                SetFootstepAudio(footStepWalkAudio);
                PlayFootStepAudio();
                m_LastStepSoundTime = Time.time;
            }
            else if (Time.time - m_LastStepSoundTime > footStepRunAudioPlayDelay && m_PlayerMotor.currentActiveSpeed2D > 0.3f)
            {
                SetFootstepAudio(footStepRunAudio);
                PlayFootStepAudio();
                m_LastStepSoundTime = Time.time;
            }
        }

        if (bPlayerWantsToAttack)
            StartAttacking();
    }

    private void FixedUpdate()
    {
        if (m_CurrentEquippedWeapon.GetAnimator() == null) return;

        if (m_PlayerMotor.IsPlayerStrafing() || m_PlayerMotor.IsPlayerWalkingBackwards())
        {
            SetCurrentAnimSpeed(0.3f);
        }
        else
        {
            SetCurrentAnimSpeed(m_PlayerMotor.currentActiveSpeed2D);
        }

        if (flashbang.isActiveAndEnabled && !flashbang.HasMoreThrowables())
            EquipPreviousWeapon();
    }

    /*
     * Equips a weapon, drops current weapon and equips the weapon picked up
     */
    public void Equip(BaseWeapon weapon)
    {
        DeactivateWeapon(m_CurrentWeaponIndex);
        weapon.OnPickup(weaponsParent);
        m_CurrentWeapons[m_CurrentWeaponIndex] = weapon;
        ActivateWeapon(m_CurrentWeaponIndex);
    }

    /*
    * Equips a weapon with teh incoming wepaon ID, drops current weapon at the drop location 
    */
    public void Equip(WeaponID weaponID, Transform dropLocation)
    {
        IPickable dropWeapon = WeaponSpawnManager.Instance.GetWeaponAlias(m_CurrentEquippedWeapon.GetWeaponID());
        dropWeapon.transform.position = dropLocation.position;

        DeactivateFlashbang();
        DeactivateWeapon(m_CurrentWeaponIndex);
        m_CurrentWeapons[m_CurrentWeaponIndex] = WeaponSpawnManager.Instance.GetWeapon(weaponID, weaponsParent.transform);
        ActivateWeapon(m_CurrentWeaponIndex);
    }

    /*
     * DEBUG_-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
     */
    public void TakeDamageTen()
    {
        TakeDamage(10);
    }

    public void EquipFlashbang()
    {
        if (!GameRunningCheck()) return;
        if (!m_CurrentEquippedWeapon.CanSwitchWeapon()) return;

        if(flashbang.isActiveAndEnabled)
        {
            if(sensor.HasMoreThrowables())
            {
                DeactivateFlashbang();
                ActivateSensor();
            }
        }
        else
        {
            if (flashbang.HasMoreThrowables())
            {
                DeactivateSensor();
                DeactivateWeapon(m_CurrentWeaponIndex);
                ActivateFlashbang();
            }
        }               
    }

    /*
     * equips weapon one
     */
    public void EquipWeaponOnePressed()
    {
        if (!GameRunningCheck()) return;

        if (flashbang.isActiveAndEnabled)
            ForceEquipWeapon(0);
        else
            EquipWeapon(0);
    }

    /*
     * equips weapon two
     */
    public void EquipWeaponTwoPressed()
    {
        if (!GameRunningCheck()) return;

        if (flashbang.isActiveAndEnabled || sensor.isActiveAndEnabled)
            ForceEquipWeapon(1);
        else
            EquipWeapon(1);
    }

    /*
     * called when player presses the button to equip next weapon
     */
    public void OnEquipWeaponOnScroll(float scrollY)
    {
        if (!GameRunningCheck()) return;

        if (scrollY > 0.0f)
            EquipNextWeapon();
        else
            EquipPreviousWeapon();
    }

    /*
     * Equips next weapon
     */
    void EquipNextWeapon()
    {
        EquipWeapon(m_CurrentWeaponIndex + 1);
    }

    /*
     * called when player presses the button to equip next weapon
     */
    public void OnEquipPreviousPressed()
    {
        if (!GameRunningCheck()) return;

        EquipPreviousWeapon();
    }

    /*
     * Equips previous weapon
     */
    void EquipPreviousWeapon()
    {
        EquipWeapon(m_CurrentWeaponIndex - 1);
    }

    /*
     * equips a weapon based on the index passed in
     */
    void EquipWeapon(int index)
    {
        if (index == m_CurrentWeaponIndex || !m_CurrentEquippedWeapon.CanSwitchWeapon()) return;

        if (flashbang.isActiveAndEnabled)
            DeactivateFlashbang();

        if (sensor.isActiveAndEnabled)
            DeactivateSensor();

        if (index == m_CurrentWeapons.Length)
            index = 0;

        if (index < 0)
            index = m_CurrentWeapons.Length - 1;

        ActivateWeapon(index);
        DeactivateWeapon(m_CurrentWeaponIndex);

        m_CurrentWeaponIndex = index;
    }

    /*
     * equips a weapon based on the index passed in, but its assumed to be a correct index, forces player to switch to the index
     */
    void ForceEquipWeapon(int index)
    {
        if (!m_CurrentEquippedWeapon.CanSwitchWeapon()) return;

        if (flashbang.isActiveAndEnabled)
            DeactivateFlashbang();

        if (sensor.isActiveAndEnabled)
            DeactivateSensor();

        ActivateWeapon(index);
        DeactivateWeapon(m_CurrentWeaponIndex);

        m_CurrentWeaponIndex = index;
    }

    /*
    * enables the weapon at @Param: 'index', and sets it to current equipped weapon
    */
    private void ActivateWeapon(int index)
    {
        m_CurrentWeapons[index].SetActive(true);
        m_CurrentEquippedWeapon = m_CurrentWeapons[index];
    }

    /*
    * enables flashbang, and sets it to current equipped weapon
    */
    private void ActivateFlashbang()
    {
        flashbang.SetActive(true);
        m_CurrentEquippedWeapon = flashbang;
    }

    private void ActivateSensor()
    {
        sensor.SetActive(true);
        m_CurrentEquippedWeapon = sensor;
    }

    /*
    * disables the weapon at @Param: 'index'
    */
    private void DeactivateWeapon(int index)
    {
        m_CurrentWeapons[index].SetActive(false);
    }

    /*
    * disables flashbang
    */
    private void DeactivateFlashbang()
    {
        flashbang.SetActive(false);
    }

    private void DeactivateSensor()
    {
        sensor.SetActive(false);
    }

    /*
     * Damages player
     * 
     * If shield is not empty, it finds the amount of damage the shield will take, which is returned by the GetShieldDamage() method,
     * then it decreases the players shield, finally decrease players health
     */
    public void TakeDamage(int damage)
    {
        int shieldDamageFallOff = 0;
        if (!IsPlayerShieldEmpty())
            shieldDamageFallOff = GetShieldDamage(damage);

        shieldDamageFallOff = DecreasePlayerShield(shieldDamageFallOff);
        DecreasePlayerHealth(damage - shieldDamageFallOff);
        healthBar.UpdateHealthBar();
        shieldBar.UpdateShieldBar();
    }

    /*
     * Decreases players health, if health goes <= zero then it calls the PlayerDied() method
     */
    void DecreasePlayerHealth(int amount)
    {
        m_CurrentPlayerHealth -= amount;

        if (m_CurrentPlayerHealth <= 0) // Player died
        {
            PlayerDied();
        }
    }

    /*
     * Increases players health, if health goes > max ealth then just sets current health to max health
     */
    public void IncreasePlayerHealth(int amount)
    {
        m_CurrentPlayerHealth += amount;

        if (m_CurrentPlayerHealth > GetPlayersMaxHealth())
            m_CurrentPlayerHealth = GetPlayersMaxHealth();

        healthBar.UpdateHealthBar();
    }

    /*
     * Decreases players shields
     * 
     * First finds out if shield can withstand the incoming amount of damage, if not, take the amount of damage the shield can handle and return that amount
     */
    int DecreasePlayerShield(int amount)
    {
        int shieldAmount = m_CurrentPlayerShield - amount;

        if (shieldAmount < 0)
            amount = m_CurrentPlayerShield;

        m_CurrentPlayerShield -= amount;

        return amount;
    }

    /*
    * Increases players shield, if shield goes > max shield then just sets current shield to max shield
    */
    public void IncreasePlayerShield(int amount)
    {
        m_CurrentPlayerShield += amount;

        if (m_CurrentPlayerShield > GetPlayersMaxShield())
            m_CurrentPlayerShield = GetPlayersMaxShield();

        shieldBar.UpdateShieldBar();
    }

    /*
     * Called when players dies 
     */
    void PlayerDied()
    {
        Debug.Log("Player died");
        Despawn();

        GameManager.Instance.ResetGame();
        AmmoManager.Instance.ResetAmmoManager();
    }

    /*
     * called when user presses attack button
     */
    public void OnAttackPressed()
    {
        if (!GameRunningCheck()) return;

        if (m_CurrentEquippedWeapon.CanPlayerHoldAttackTrigger())
            bPlayerWantsToAttack = true;

        StartAttacking();
    }

    /*
     * called when users holds down the attack button
     */
    public void OnAttackHold()
    {
        if (!GameRunningCheck()) return;

        if (m_CurrentEquippedWeapon.CanPlayerHoldAttackTrigger())
            StartAttacking();
    }

    public void OnAttackReleased()
    {
        bPlayerWantsToAttack = false;
        //m_CurrentEquippedWeapon.StopAttacking();
    }

    /*
     * called when user presses ads button
     */
    public void OnADSPressed()
    {
        if (!GameRunningCheck()) return;

        if (!m_CurrentEquippedWeapon.CanPlayerHoldAimTrigger() && m_CurrentEquippedWeapon.IsWeaponAimed())
            StopAiming();
        else
            StartAiming();
    }

    /*
    * called when user releases ads button
    */
    public void OnADSReleased()
    {
        if (!GameRunningCheck()) return;

        if (m_CurrentEquippedWeapon.CanPlayerHoldAimTrigger())
            StopAiming();
    }

    /*
     * called when user presses reload button
     */
    public void OnReloadPressed()
    {
        if (!GameRunningCheck()) return;

        StartReloading();
    }

    /*
     * starts weapon shooting
     */
    void StartAttacking()
    {
        m_CurrentEquippedWeapon.StartAttacking();
        UpdateFlashbangCount();
        UpdateSensorGrenadeUi();
    }

    /*
     * starts weapon reloading
     */
    void StartReloading()
    {
        m_CurrentEquippedWeapon.Reload();
    }

    /*
     * starts weapon aiming
     */
    void StartAiming()
    {
        m_CurrentEquippedWeapon.StartAiming();
        m_PlayerMotor.SlowWalk(true);
        m_PlayerMotor.CheckSlowWalk();
    }

    /*
     * stops weapon aiming
     */
    void StopAiming()
    {
        m_CurrentEquippedWeapon.StopAiming();
        m_PlayerMotor.SlowWalk(false);
        m_PlayerMotor.CheckSlowWalk();
    }

    /*
    * lerps from last aniamtion speed to current animation speed to create a flow
    */
    public void SetCurrentAnimSpeed(float speed)
    {
        m_LastAnimSpeed = Mathf.Lerp(m_LastAnimSpeed, speed, animationSpeedInterpTime);
        m_CurrentEquippedWeapon.SetAnimFloat("Velocity", m_LastAnimSpeed);
    }

    /*
    * Update the ammo gui text
    */
    public void UpdateAmmoGUI()
    {
        m_CurrentEquippedWeapon.UpdateAmmoGUI();
    }

    /*
    * returns players max health
    */
    public int GetPlayersMaxHealth()
    {
        return maxPlayerHealth;
    }

    /*
    * returns players current health
    */
    public int GetCurrentPlayerHealth()
    {
        return m_CurrentPlayerHealth;
    }

    /*
    * returns players max shield
    */
    public int GetPlayersMaxShield()
    {
        return maxPlayerShield;
    }

    /*
    * returns if players shield has ran out
    */
    bool IsPlayerShieldEmpty()
    {
        return m_CurrentPlayerShield <= 0;
    }

    /*
    * returns players current shield amount
    */
    public int GetCurrentPlayerShield()
    {
        return m_CurrentPlayerShield;
    }

    /*
    * returns the amount of shield needed to be taken off upon some damage
    */
    int GetShieldDamage(int damageTaken)
    {
        return (int)(damageTaken * playerShieldFallOffScale);
    }

    /*
    * set the footstep audio sources clip to the @Param: 'clip'
    */
    void SetFootstepAudio(AudioClip clip)
    {
        m_FootstepAudioSrc.clip = clip;
    }

    /*
    * plays a footstep audio
    */
    void PlayFootStepAudio()
    {
        m_FootstepAudioSrc.Play();
    }

    public int GetCurrentFlashbangsAmount()
    {
        return flashbang.GetCurrentAmountOfThrowables();
    }

    public int GetMaxFlashBangs()
    {
        return flashbang.GetMaxAmountOfThrowables();
    }

    bool GameRunningCheck()
    {
        return Time.timeScale > 0f;
    }

    public void UpdateFlashbangCount()
    {
        GameManager.Instance.flashBangCount.text = flashbang.GetCurrentAmountOfThrowables().ToString();
    }

    public void FlashPlayer()
    {
        StartCoroutine(FadeOutFlashbangImage());
    }

    IEnumerator FadeOutFlashbangImage()
    {
        Color color = Color.white;

        for (float a = 1f; a > 0; a -= Time.deltaTime * flashbangTimeDamp)
        {
            color.a = a;
            GameManager.Instance.flashbangImage.color = color;

            yield return null;
        }
    }

    public void PlayOneShotAudio(AudioClip clip)
    {
        weaponAudioSrc.PlayOneShot(clip);
    }

    public bool CanPickupWeapon()
    {
        return m_CurrentEquippedWeapon.CanSwitchWeapon();
    }

    public void IncreaseFlashbang(int amount)
    {
        flashbang.IncreaseThrowable(amount);
    }

    public void SetCurrentRecoilIndex(int index)
    {
        m_CurrentEquippedWeapon.SetRecoilPatternIndex(index);
    }

    public int GetCurrentRecoilIndex()
    {
        return m_CurrentEquippedWeapon.GetRecoilPatternIndex();
    }

    public void IncreaseSensorGrenade(int amount)
    {
        sensor.IncreaseThrowable(amount);
    }
    public int GetCurrentSensorGrenadeCount()
    {
        return sensor.GetCurrentAmountOfThrowables();
    }

    public int GetMaxSensorGrenadeCount()
    {
        return sensor.GetMaxAmountOfThrowables();
    }

    public void UpdateSensorGrenadeUi()
    {
        GameManager.Instance.sensorGrenadeCount.text = sensor.GetCurrentAmountOfThrowables().ToString();
    }

    public BaseWeapon GetCurrentEquippedWeapon()
    {
        return m_CurrentEquippedWeapon;
    }

    public BaseWeapon GetCurrentEquippedGun()
    {
        for(int i = 0; i < m_CurrentWeapons.Length; i++)
        {
            if (m_CurrentWeapons[i].GetWeaponID() != WeaponID.Knife)
            {
                return m_CurrentWeapons[i];
            }
        }

        return null;
    }
}
