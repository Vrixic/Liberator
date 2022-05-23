using System.Collections;
using UnityEngine;

public class Explodable : ISpawnable
{
    [Header("Particle Effects")]
    [SerializeField] PoolableObject fire;
    [SerializeField] PoolableObject explosion;

    [Header("Sound Effects")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip fireSound;
    [SerializeField] AudioClip exploSound;

    string m_ExplosionPool;
    string m_FirePool;

    int hits = 0;
    int counter = 0;
    [SerializeField] float radius;
    float damage;

    bool isExploded = false;
    bool firstHit = false;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void Start()
    {
        m_ExplosionPool = ObjectPoolManager.CreateObjectPool(explosion, 1);
        m_FirePool = ObjectPoolManager.CreateObjectPool(fire, 3);
    }

    // Called when a bullet hits an explodable, starts spewing fire from location of hit
    public void ExplodableIsHit(Vector3 pos, Vector3 forward)
    {
        if (!isExploded)
        {
            StartCoroutine(TimeTillExplode());
            if (!firstHit)
            {
                audioSource.clip = fireSound;
                audioSource.Play();
                firstHit = true;
            }

            fire = ObjectPoolManager.SpawnObject(m_FirePool);
            fire.transform.position = pos;
            fire.transform.forward = (Vector3.up + forward).normalized;

            hits++;
        }
    }

    // checks if enemy or player was hit by explosion and damages them according
    // to distance away from explosion
    public void ExplodeDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].tag == "Player"){
                float dist = Vector3.Distance(colliders[i].transform.position, transform.position);
                damage = 300 / (dist + 0.1f);
                GameManager.Instance.playerScript.TakeDamage((int)damage);
            }
            else if (colliders[i].tag == "Hitbox"){
                float dist = Vector3.Distance(colliders[i].transform.position, transform.position);
                damage = 300 / (dist + 0.1f);
                colliders[i].GetComponent<Health>().TakeDamage((int)damage, transform.position);
            }
        }
    }

    // checking for when to cause the explosion, 3 sec after being hit, or after 3 hits
    private void Update()
    {
        if ((counter == 3 || hits == 3) && !isExploded)
        {
            ObjectPoolManager.DisableAllInPool(m_FirePool);
            isExploded = true;
            explosion = ObjectPoolManager.SpawnObject(m_ExplosionPool);
            explosion.transform.position = transform.position;

            transform.GetChild(0).gameObject.SetActive(false);

            audioSource.clip = exploSound;
            audioSource.Play();

            ExplodeDamage();
        }
        if (isExploded && counter > 3)
        {
            StopCoroutine(TimeTillExplode());
            Despawn();
        }
    }

    // despawns the explodable after explosion
    public override void Despawn()
    {
        base.Despawn();

        gameObject.SetActive(false);
    }

    //respawns explodable on restart
    public override void Respawn()
    {
        base.Respawn();

        hits = 0;
        counter = 0;
    }

    //counter for when to explode the tank
    IEnumerator TimeTillExplode()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            counter++;
        }
    }
}
