using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    AIAgent agent;
    [SerializeField] [Tooltip("amount of times to attack per second")] float attackRate = 1f;
    [SerializeField] int damage = 50;
    float m_LastAttackTime = 0f;


    private void Start()
    {
        m_LastAttackTime = 0f;
        attackRate = 1f / attackRate;
        agent = GetComponent<AIAgent>();
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

    public void DealDamage()
    {
        if (agent.sqrDistance <= 6)
        {
            DISystem.createIndicator(transform);
            GameManager.Instance.playerScript.TakeDamage(damage);

            //add camerashake for flinch
            GameManager.Instance.cameraShakeScript.Trauma = 1f;
        }
    }

    bool CanAttack()
    {
        return Time.time > (m_LastAttackTime + attackRate);
    }
}
