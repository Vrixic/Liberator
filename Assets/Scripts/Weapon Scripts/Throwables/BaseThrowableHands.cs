using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseThrowableHands : BaseWeapon
{
    /* scales the force being applied to the flash bang */
    [SerializeField] [Tooltip("scales the force being applied to the flash bang")] float throwForceMultiplier = 10.0f;

    /* the amount of distance the Throwable should go to before exploding */
    [SerializeField] [Tooltip("the amount of distance the Throwable should go to before exploding")] float distanceFromPlayerMultiplier = 10.0f;

    /* The flash bang prefab used to create the object pool of Throwables */
    [SerializeField] [Tooltip("The flash bang prefab used to create the object pool of Throwables")] BaseThrowables throwablePrefab;

    /* max amount of Throwables the player can hold at a given time */
    [SerializeField] [Tooltip("max amount of Throwables the player can hold at a given time")] int maxThrowableAmount = 2;

    [SerializeField] public GameObject throwableMesh;

    /* current amount of Throwables */
    int m_CurrentThrowableAmount;

    /* object pool of Throwables */
    private string m_ThrowablePool;

    public BaseThrowableHands()
    {
        m_CurrentThrowableAmount = 1;
    }

    public override void Start()
    {
        base.Start();
        m_ThrowablePool = ObjectPoolManager.Instance.CreateObjectPool(throwablePrefab, maxThrowableAmount);
        m_CurrentThrowableAmount = maxThrowableAmount;
    }

    public override void OnWeaponEquip()
    {
        //base.OnWeaponEquip();
        if (AmmoManager.Instance != null)
            AmmoManager.Instance.HideAmmoGUI();
    }

    public override void OnWeaponUnequip()
    { }

    public void OnAnimationEvent_ThrowStart()
    {
        throwableMesh.SetActive(false);
    }

    /*
    * Called when the player wants to throw the Throwable
    *   - if player has mroe Throwables and the attack delay is met then it allows to throw the Throwable
    *   - turn bIsThrowing on
    *   - the it plays the attack audio
    *   - Sets the animator to play the throwing animation and updates the last attack time
    *   - Decrements the current amount of Throwables left
    */
    public override void StartAttacking()
    {
        if (!HasMoreThrowables()) return;

        if (TakeAction(m_LastAttackTime, attackRate))
        {

            GetAnimator().Play("Attack1");
            UpdateLastAttackTime();

            m_CurrentThrowableAmount--;
        }
    }

    public void OnAnimationEvent_ThrowEnd()
    {
        throwableMesh.SetActive(true);
    }

    /*
    * Called when the throwing animation is at where the Throwable should be throw'd
    *   - spawns a Throwable from the Throwable pool
    *   - finds the targetpoint the Throwable needs to reach
    *   - calls the Throwables OnThrowThowable() method to let the Throwable know player is throwing  it
    */
    public void OnAnimationEvent_Throw()
    {
        BaseThrowables throwable = ObjectPoolManager.Instance.SpawnObject(m_ThrowablePool) as BaseThrowables;
        throwable.transform.position = raycastOrigin.position;
        throwable.transform.forward = raycastOrigin.forward;

        Vector3 targetPoint = GameManager.Instance.mainCamera.transform.position + GameManager.Instance.mainCamera.transform.forward * distanceFromPlayerMultiplier;
        Vector3 direction = (targetPoint - raycastOrigin.position).normalized;

        //Debug.DrawLine(raycastOrigin.position, targetPoint, Color.red, 2f);

        throwable.OnThrowThrowable(direction, throwForceMultiplier);
    }

    public void IncreaseThrowable(int amount)
    {
        m_CurrentThrowableAmount = amount;

        if (m_CurrentThrowableAmount > maxThrowableAmount)
            m_CurrentThrowableAmount = maxThrowableAmount;
    }

    /*
    * returns if player can switch from the Throwable to another weapon
    */
    public override bool CanSwitchWeapon()
    {
        return !bIsAttacking;
    }

    /*
    * returns if theirs any Throwables left
    */
    public bool HasMoreThrowables()
    {
        return m_CurrentThrowableAmount > 0;
    }

    /*
    * returns the max amount of Throwables 
    */
    public int GetMaxAmountOfThrowables()
    {
        return maxThrowableAmount;
    }

    public void SetMaxAmountOfThrowables(int newThrowableCapacity)
    {
        maxThrowableAmount = newThrowableCapacity;
    }

    /*
    * returns the current amount of Throwables left 
    */
    public int GetCurrentAmountOfThrowables()
    {
        return m_CurrentThrowableAmount;
    }
}
