using UnityEngine;

public class BaseWeapon : ISpawnable //, IWeapon
{
    [Header("BaseWeapon")]

    [Header("Player Settings")]

    /* bool to keep in track if player can hold down the attack button */
    [SerializeField] protected bool allowPlayerToHoldAttackTrigger = false;

    /* bool to keep in track if player can hold down the aim button */
    [SerializeField] protected bool allowPlayerToHoldAimTrigger = false;

    [Header("Delays")]

    /* amount of this weapon can be attacked in the spawn of 1 second */
    [SerializeField] [Tooltip("amount of this weapon can be attacked in the spawn of 1 second")] protected float attackRate = 1.0f;

    [Header("Weapon Settings")]

    /* origin of the raycast done when this weapon starts attacking */
    [SerializeField] protected Transform raycastOrigin;

    [SerializeField] protected int damage = 25;

    [SerializeField] Vector3 weaponSpawnLocalPositionOffset;

    [SerializeField] [Tooltip("Raycast will only query these layers")] protected LayerMask raycastLayers;

    [Header("Audio Settings")]

    /* audio clip played when player attacks */
    [SerializeField] AudioClip attackAudioClip;

    protected bool bIsAttacking;

    /* if player has picked up this weapon and its in his inventory */
    protected bool bIsPickedUp;

    /* time when weapon was last fired */
    protected float m_LastAttackTime;

    protected Animator m_Animator;

    WeaponID m_WeaponID;

    public virtual void Start()
    {
        attackRate = 1 / attackRate;
       
        m_Animator = GetComponent<Animator>();

        Spawn();
    }

    public override void Spawn()
    {
        base.Spawn();

        m_LastAttackTime = attackRate * -1;
    }

    private void OnEnable()
    {
        OnWeaponEquip();
    }

    private void OnDisable()
    {
        OnWeaponUnequip();
    }

    public virtual void OnPickup(GameObject parent) 
    {
        //Debug.Log(parent.name + " just picked up " + name);
        bIsPickedUp = true;

        transform.parent = parent.transform;
        transform.localPosition = weaponSpawnLocalPositionOffset; 
    }

    public virtual void OnDrop(Transform dropLocation)
    {
        bIsPickedUp = false;

        transform.parent = null;
        transform.position = dropLocation.position;
    }

    public virtual void OnWeaponEquip() { }

    public virtual void OnWeaponUnequip() { }

    /*
    * for override purposes
    */
    public virtual void Update() { }

    /*
    * updates the last attack time
    */
    public virtual void StartAttacking() { bIsAttacking = true; }

    public virtual void OnAnimationEvent_AttackStart() { bIsAttacking = true; }

    public virtual void OnAnimationEvent_AttackEnd() { bIsAttacking = false; }

    public virtual void StopAttacking() { bIsAttacking = false; }

    /*
    * for override purposes
    */
    public virtual void Reload() { }

    public virtual void OnAnimationEvent_ReloadStart() { }
    public virtual void OnAnimationEvent_ReloadEnd() { }

    /*
    * for override purposes
    */
    public virtual void StartAiming() { }

    /*
    * for override purposes
    */
    public virtual void StopAiming() { }

    /*
    * for override purposes
    */
    public virtual void UpdateAmmoGUI() { }

    /*
    * returns if player can switch to another wepaon
    */
    public virtual bool CanSwitchWeapon() { return false; }

    /*
    * updates the last attack time
    */
    public void UpdateLastAttackTime()
    {
        m_LastAttackTime = Time.time;
    }
    
    /*
    * returns if the weapon is reloading
    */
    public virtual bool IsWeaponReloading()
    {
        return false;
    }

    /*
    * returns if the weapon is aimed
    */
    public virtual bool IsWeaponAimed()
    {
        return false;
    }

    /*
    * returns if player can hold down attack trigger
    */
    public bool CanPlayerHoldAttackTrigger()
    {
        return allowPlayerToHoldAttackTrigger;
    }

    /*
    * returns if player can hold down aim trigger
    */
    public bool CanPlayerHoldAimTrigger()
    {
        return allowPlayerToHoldAimTrigger;
    }

    /*
    * returns the amount of damage the weapon will do
    */
    public int GetDamage()
    {
        return damage;
    }

    /*
    * returns last attack time
    */
    public float GetLastAttackTime()
    {
        return m_LastAttackTime;
    }

    /*
     * Returns if object should take action depending on delay is exceeded
     */
    protected bool TakeAction(float lastTime, float delay)
    {
        return (lastTime + delay) < Time.time;
    }

    /*
    * enables/disables gameObject
    */
    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    /*
    * plays the audio 
    */
    protected void PlayAttackAudio()
    {
        //m_AudioSource.PlayOneShot(attackAudioClip);
        GameManager.Instance.playerScript.PlayOneShotAudio(attackAudioClip);
    }

    /*
    * plays the audio 
    */
    protected void PlayAudioOneShot(AudioClip clip)
    {
        //m_AudioSource.PlayOneShot(clip);
        GameManager.Instance.playerScript.PlayOneShotAudio(clip);
    }

    public void SetAnimator(Animator animator)
    {
        m_Animator = animator;
    }

    public Animator GetAnimator()
    {
        return m_Animator;
    }

    public bool IsAttacking()
    {
        return bIsAttacking;
    }

    public void SetAnimFloat(string name, float num)
    {
        m_Animator.SetFloat(name, num);
    }

    public void SetWeaponID(WeaponID id)
    {
        m_WeaponID = id;
    }

    public WeaponID GetWeaponID()
    {
        return m_WeaponID;
    }

    public virtual void SetRecoilPatternIndex(int i)
    {
       
    }

    public virtual int GetRecoilPatternIndex()
    {
        return 0;
    }
}
