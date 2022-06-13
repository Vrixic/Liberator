using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EnemyMelee : MonoBehaviour
{
    [SerializeField] [Tooltip("amount of times to attack per second")] float attackRate = 1f;
    [SerializeField] int damage = 50;
    float m_LastAttackTime = 0f;

    BoxCollider m_SphereCollider;

    /* if player has been attacked once already, prevent infinite damage on one hit */
    bool bHasAttacked = false;

    private void Start()
    {
        m_LastAttackTime = 0f;
        attackRate = 1f / attackRate;

        m_SphereCollider = GetComponent<BoxCollider>();
        m_SphereCollider.isTrigger = true;
        m_SphereCollider.enabled = false;
    }

    /* returns if enemy can attack based on the last time the enemy attacked */
    public bool Attack()
    {
        if (CanAttack())
        {
            m_LastAttackTime = Time.time;

            return true;
        }

        return false;
    }

    /* turns the trigger on */
    public void OnAnimationEvent_AttackStart()
    {
        m_SphereCollider.enabled = true;
        bHasAttacked = false;
    }


    /* turns the trigger off */
    public void OnAnimationEvent_AttackEnd()
    {
        m_SphereCollider.enabled = false;
    }

    /* damages the player if, player was in range of the melee attack */
    private void OnTriggerEnter(Collider other)
    {
        if (!bHasAttacked)
        {
            if (other.CompareTag("Player"))
            {
                DISystem.createIndicator(transform);
                

                //add camerashake for flinch
                GameManager.Instance.cameraShakeScript.Trauma += 1f;

                bHasAttacked = true;
            }
        }
    }

    public void DealDamage()
    {
        GameManager.Instance.playerScript.TakeDamage(damage);
    }
    private void OnTriggerExit(Collider other)
    {
        bHasAttacked = false;
        m_SphereCollider.enabled = false;
    }

    bool CanAttack()
    {
        return Time.time > (m_LastAttackTime + attackRate);
    }
}
