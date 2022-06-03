using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    [SerializeField] [Tooltip("amount of this weapon can be attacked in the spawn of 1 second")] float fireRate = 1.0f;

    [SerializeField] int damage = 12;

    /* prefab of the bullet that will be spawned in */
    [SerializeField] Bullet bulletPrefab;

    [SerializeField] Transform bulletSpawnLocation;

    /* max number of bullets this gun can have at one time */
    [SerializeField] int maxNumOfBullets = 30;

    [SerializeField] [Tooltip("Squared range")] public float bulletRange = 10000f;

    [SerializeField] float reloadTime = 2.0f;

    /* current number of bullets */
    int m_CurrentNumOfBullets;

    /* time when weapon was last fired */
    protected float m_NextTimeToFire;

    /* a objectpool that will keep track of spawned bullets automatically */
    string m_BulletPool;

    /*
   * Creates the bullet pool 
   */
    public void Start()
    {
        fireRate = 1 / fireRate;
        m_CurrentNumOfBullets = maxNumOfBullets;

        m_BulletPool = ObjectPoolManager.Instance.CreateObjectPool(bulletPrefab, maxNumOfBullets);
    }

    public void Shoot()
    {
        if (IsGunEmpty()) return;

        if (Time.time > m_NextTimeToFire)
        {
            ShootBullet(bulletSpawnLocation.forward);

            m_NextTimeToFire = Time.time + fireRate;

            m_CurrentNumOfBullets--;
        }

        if (m_CurrentNumOfBullets < 1)
        {
            Reload();
        }
    }

    public void Shoot(Vector3 forward)
    {
        if (Time.time > m_NextTimeToFire)
        {
            ShootBullet(forward);

            m_NextTimeToFire = Time.time + fireRate;

            m_CurrentNumOfBullets--;
        }

        if (m_CurrentNumOfBullets < 1)
        {
            Reload();
        }
    }


    public bool ShootAtTarget(Vector3 target, Vector2 radius)
    {
        if (IsGunEmpty() || Time.time < m_NextTimeToFire) return false;

        m_NextTimeToFire = Time.time + fireRate;

        Vector3 newTarget = Vector3.zero;
        newTarget.x = Random.Range(target.x - radius.x, target.x + radius.x);
        newTarget.y = Random.Range(target.y - radius.y, target.y + radius.y);
        newTarget.z = Random.Range(target.z - radius.x, target.z + radius.x);

        ShootAtTarget(newTarget);

        return true;
    }

    public void ShootAtTarget(Vector3 target)
    {
        Vector3 direction = (target - bulletSpawnLocation.position).normalized;

        ShootBullet(direction);

        m_CurrentNumOfBullets--;

        if (m_CurrentNumOfBullets < 1)
        {
            Reload();
        }
    }

    public void ShootBullet(Vector3 direction)
    {
        Bullet bullet = ObjectPoolManager.Instance.SpawnObject(m_BulletPool) as Bullet;

        RaycastHit hitInfo;
        if (Physics.Raycast(bulletSpawnLocation.position, direction, out hitInfo, bulletRange))
        {
            float volume = 0.5f;
            if (hitInfo.collider.CompareTag("Player"))
            {
                GameManager.Instance.playerScript.TakeDamage(damage);

                //create damage indicator UI
                DISystem.createIndicator(transform);

                volume = 0.125f;
            }
            
            bullet.Spawn(bulletSpawnLocation.position, direction, hitInfo, volume);
        }
        else
        {
            bullet.Spawn(bulletSpawnLocation.position, direction, bulletRange);
        }
    }

    public void Reload()
    {
        m_CurrentNumOfBullets = maxNumOfBullets;
        m_NextTimeToFire += reloadTime;
    }

    public bool IsGunEmpty()
    {
        return m_CurrentNumOfBullets < 1;
    }

    public void ResetGun()
    {
        m_CurrentNumOfBullets = maxNumOfBullets;
        m_NextTimeToFire = 0;
    }
}
