using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Weapon Settings")]

    /* array of weapons */
    [SerializeField] WeaponID[] startWeaponIDs = new WeaponID[2];
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
     int maxPlayerHealth = 100;

    /* max health player can have */
     int maxPlayerShield = 100;

    /* amount of percentage of damage the shield can intake when player has been damaged */
    [SerializeField] float playerShieldFallOffScale = 0.8f;

    [Header("Animation Settings")]

    /* time it takes to lerp from last animation speed to current animation speed */
    [SerializeField] float animationSpeedInterpTime = 0.1f;

    [Header("Audio Settings")]

    [SerializeField] public AudioSource weaponAudioSrc;

    /* current equipped weapon */
    int m_CurrentWeaponIndex;

    /* current player health */
    int m_CurrentPlayerHealth;

    /* current player shield */
    int m_CurrentPlayerShield;

    /* Last Animation speed */
    float m_LastAnimSpeed = 0f;

    public float equipmentTimer = 100f;

    /* audio source used for audio on player */
    AudioSource m_PlayerAudioSrc;

    float m_DefaultVolume = 1f;

    /* player motor of the player */
    PlayerMotor m_PlayerMotor;

    /* current equipped weapon */
    BaseWeapon m_CurrentEquippedWeapon;

    /* Health bar connected to player, in inspector must select health bar from ui to work*/
    HealthBar healthBar;

    /* Shield bar connected to player, in inspector must select Shield bar from ui to work*/
    ShieldBar shieldBar;

    bool bPlayerWantsToAttack = false;

    bool bPlayerDead = false;

    bool godMode = false;



    CharacterController characterController;

    private void Start()
    {
        m_PlayerMotor = GetComponent<PlayerMotor>();
        m_PlayerAudioSrc = GetComponentInChildren<AudioSource>();
        characterController = GetComponent<CharacterController>();

        m_DefaultVolume = m_PlayerAudioSrc.volume;

        healthBar = GameManager.Instance.healthBarScript;
        shieldBar = GameManager.Instance.shieldBarScript;

        m_CurrentWeapons = new BaseWeapon[startWeaponIDs.Length];
        m_CurrentWeaponIndex = 0;

        for (int i = 0; i < startWeaponIDs.Length; i++)
        {
            m_CurrentWeapons[i] = WeaponSpawnManager.Instance.GetWeapon(startWeaponIDs[i], weaponsParent.transform);
            //m_CurrentWeapons[i].OnPickup(weaponsParent);
        }

        //ActivateWeapon(0);
        //DeactivateWeapon(1);

        // Set capcity of flashbangs and sensor grenades to Capacity dictated by Player Prefs
        flashbang.SetMaxAmountOfThrowables(PlayerPrefManager.Instance.flashBangCapacity);
        flashbang.IncreaseThrowable(PlayerPrefManager.Instance.flashBangCapacity);

        sensor.SetMaxAmountOfThrowables(PlayerPrefManager.Instance.sensorGrenadeCapacity);
        sensor.IncreaseThrowable(PlayerPrefManager.Instance.sensorGrenadeCapacity);


        SetEquipmentEffectivness();

        UpdateFlashbangCount();
        UpdateSensorGrenadeUi();
        maxPlayerHealth = PlayerPrefManager.Instance.playerStartingHealth;
        maxPlayerShield = PlayerPrefManager.Instance.playerStartingArmor;
        ResetHealth();

        flashbang.OnPickup(weaponsParent);
        sensor.OnPickup(weaponsParent);

        DeactivateFlashbang();
        DeactivateSensor();

        OnOptionsUpdate();

        ActivateWeapon(0);
        DeactivateWeapon(1);
    }
    void SetEquipmentEffectivness()
    {
        flashbang.SetSphereRadius();
        sensor.SetSphereRadius();
        equipmentTimer = PlayerPrefManager.Instance.equipmentEffectiveness;
        sensor.SetEquipmentTimer();
    }

    void ResetHealth()
    {
        m_CurrentPlayerHealth = maxPlayerHealth;
        m_CurrentPlayerShield = maxPlayerShield;
        healthBar.SetMaxHealth();
        shieldBar.SetMaxShield();
        healthBar.UpdateHealthBar();
        shieldBar.UpdateShieldBar();
    }

    private void Update()
    {
        if (bPlayerDead) return;
        if (godMode)
        {
            if (m_CurrentPlayerHealth <= 100)
            {
                Debug.Log("Health too low, Resetting");
                m_CurrentPlayerHealth = 999999999;
            }
        }
    }

    private void FixedUpdate()
    {
        if (bPlayerDead) return;

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

        if (sensor.isActiveAndEnabled && !sensor.HasMoreThrowables())
            EquipPreviousWeapon();

        if (bPlayerWantsToAttack)
            StartAttacking();
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
    public void Equip(WeaponID weaponID)
    {
        DeactivateSensor();
        DeactivateFlashbang();
        DeactivateWeapon(m_CurrentWeaponIndex);

        m_CurrentWeaponIndex = 1;

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

        //if (flashbang.isActiveAndEnabled)
        //{
        //    if (sensor.HasMoreThrowables())
        //    {
        //        DeactivateFlashbang();
        //        ActivateSensor();
        //    }
        //}
        //else
        //{
        //    if (flashbang.HasMoreThrowables())
        //    {
        //        DeactivateSensor();
        //        DeactivateWeapon(m_CurrentWeaponIndex);
        //        ActivateFlashbang();
        //    }
        //    else
        //    {
        //        if (sensor.HasMoreThrowables())
        //        {
        //            DeactivateWeapon(m_CurrentWeaponIndex);
        //            ActivateSensor();
        //        }
        //    }
        //}

        if (flashbang.HasMoreThrowables())
        {
         //   DeactivateSensor();
            DeactivateWeapon(m_CurrentWeaponIndex);
            ActivateFlashbang();
        }
    }

    public void EquipSensor()
    {
        if (!GameRunningCheck()) return;
        if (!m_CurrentEquippedWeapon.CanSwitchWeapon()) return;

        if (sensor.HasMoreThrowables())
        {
            DeactivateWeapon(m_CurrentWeaponIndex);
            ActivateSensor();
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
    public void EquipNextWeapon()
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
    public void ForceEquipWeapon(int index)
    {
        if (!m_CurrentEquippedWeapon.CanSwitchWeapon()) return;

        if (flashbang.isActiveAndEnabled)
            DeactivateFlashbang();

        if (sensor.isActiveAndEnabled)
            DeactivateSensor();

        ActivateWeapon(index);
        //DeactivateWeapon(m_CurrentWeaponIndex);

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
        if (sensor.isActiveAndEnabled)
            DeactivateSensor();

        flashbang.SetActive(true);
        m_CurrentEquippedWeapon = flashbang;
    }

    private void ActivateSensor()
    {
        if (flashbang.isActiveAndEnabled)
            DeactivateFlashbang();

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

    // enables flashbang mesh
    public void ActivateFlashbangMesh()
    {
        flashbang.throwableMesh.SetActive(true);
    }

    public void ActivateSensorMesh()
    {
        sensor.throwableMesh.SetActive(true);
    }

    /*
     * Damages player
     * 
     * If shield is not empty, it finds the amount of damage the shield will take, which is returned by the GetShieldDamage() method,
     * then it decreases the players shield, finally decrease players health
     */
    public void TakeDamage(int damage)
    {
        if (bPlayerDead) return;

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
        if (!GameRunningCheck()) return;
        m_CurrentPlayerHealth -= amount;
        if (m_CurrentPlayerHealth <= 30)
        {
            AudioManager.Instance.ResumeMusic();
        }
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
        if (m_CurrentPlayerHealth > 30)
        {
            AudioManager.Instance.PauseMusic();
        }
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
        AudioManager.Instance.StopMusic();

        GameManager.Instance.damageIndicatorSystem.ClearAllIndicators();

        characterController.enabled = false;
        bPlayerDead = true;

        StartCoroutine(PlayPlayerDeath());
    }

    IEnumerator PlayPlayerDeath()
    {
        float time = 1f;
        while (Time.timeScale > 0)
        {
            time -= 1f * Time.unscaledDeltaTime;
            time = Mathf.Clamp(time, 0f, 1f);
            Time.timeScale = time;
            transform.Rotate(-95 * Time.unscaledDeltaTime, 0, 0);
            yield return null;
        }
        GameManager.Instance.GameWon = false;

        DeactivateWeapon(m_CurrentWeaponIndex);
        DeactivateSensor();
        DeactivateFlashbang();

        GameManager.Instance.ResetGame();

        AmmoManager.Instance.ResetAmmoManager();

        //SceneManager.LoadScene(0);
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
        //if (!GameRunningCheck()) return;

        //if (m_CurrentEquippedWeapon.CanPlayerHoldAttackTrigger())
        //    StartAttacking();
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

        if (m_CurrentEquippedWeapon.GetWeaponID() == WeaponID.Knife) return;

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
    public void ToggleGodMode()
    {
        if (!godMode)
        {

            m_CurrentPlayerHealth = 999999999;

            Debug.Log("God Mode On");
            godMode = true;
        }
        else
        {
            ResetHealth();
            godMode = false;
            Debug.Log("God Mode off");
        }
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
        return Time.timeScale > 0f && !GameManager.Instance.IsUIOverlayVisible;
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
        weaponAudioSrc.volume = PlayerPrefManager.Instance.sfxVolume / 100;
        weaponAudioSrc.PlayOneShot(clip);
    }

    public bool CanPickupWeapon()
    {
        return m_CurrentEquippedWeapon.CanSwitchWeapon();
    }

    public void IncreaseFlashbangAmount(int amount)
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

    public void IncreaseSensorGrenadeAmount(int amount)
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
        for (int i = 0; i < m_CurrentWeapons.Length; i++)
        {
            if (m_CurrentWeapons[i].GetWeaponID() != WeaponID.Knife)
            {
                return m_CurrentWeapons[i];
            }
        }

        return null;
    }

    //public void ResetSave()
    //{
    //    PlayerPrefs.SetInt("SecondaryGun", (int)startWeaponIDs[1]);
    //    PlayerPrefs.SetInt("Health", GetCurrentPlayerHealth());
    //    PlayerPrefs.SetInt("Shield", GetCurrentPlayerShield());
    //}

    //public void SaveGame()
    //{
    //    PlayerPrefs.SetInt("SecondaryGun", (int)m_CurrentWeapons[1].GetWeaponID());
    //    PlayerPrefs.SetInt("Health", GetPlayersMaxHealth());
    //    PlayerPrefs.SetInt("Shield", GetPlayersMaxShield());
    //}

    //public void LoadGame()
    //{
    //    if(PlayerPrefs.HasKey("SecondaryGun"))
    //    {
    //        m_CurrentWeapons[1] = WeaponSpawnManager.Instance.GetWeapon((WeaponID)PlayerPrefs.GetInt("SecondaryGun"), weaponsParent.transform);
    //    }
    //    else
    //    {
    //        PlayerPrefs.SetInt("SecondaryGun", (int)m_CurrentWeapons[1].GetWeaponID());
    //    }

    //    if (PlayerPrefs.HasKey("Health"))
    //    {
    //        m_CurrentPlayerHealth = PlayerPrefs.GetInt("Health");
    //    }
    //    else
    //    {
    //        PlayerPrefs.SetInt("Health", GetPlayersMaxHealth());
    //    }


    //    if (PlayerPrefs.HasKey("Shield"))
    //    {
    //        m_CurrentPlayerHealth = PlayerPrefs.GetInt("Shield");
    //    }
    //    else
    //    {
    //        PlayerPrefs.SetInt("Health", GetPlayersMaxHealth());
    //    }
    //}

    /* When user saves the game, updates variables to the recent saves */
    public void OnOptionsUpdate()
    {
        m_PlayerAudioSrc.volume = m_DefaultVolume * PlayerPrefManager.Instance.masterVolume;
        weaponAudioSrc.volume = m_DefaultVolume * PlayerPrefManager.Instance.masterVolume;
    }

    //private void OnApplicationQuit()
    //{
    //    ResetSave();
    //}
}
