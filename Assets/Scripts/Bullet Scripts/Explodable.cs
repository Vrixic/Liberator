using System.Collections;
using UnityEngine;

public class Explodable : MonoBehaviour
{
    [Header("Particle Effects")]
    [SerializeField] PoolableObject fire;
    [SerializeField] PoolableObject explosion;

    [Header("Sound Effects")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip fireSound;
    [SerializeField] AudioClip exploSound;

    [SerializeField] LayerMask layers;

    PoolableObject[] m_FirePooled;

    Transform m_Mesh;

    string m_ExplosionPool;
    string m_FirePool;

    int hits = 0;

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
        m_ExplosionPool = ObjectPoolManager.Instance.CreateObjectPool(explosion, 1);
        m_FirePool = ObjectPoolManager.Instance.CreateObjectPool(fire, 3);

        m_FirePooled = new PoolableObject[3];

        m_Mesh = transform.GetChild(0);
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

            fire = ObjectPoolManager.Instance.SpawnObject(m_FirePool);
            fire.transform.position = pos;
            fire.transform.forward = (Vector3.up + forward).normalized;
            m_FirePooled[hits] = fire;

            hits++;
            if (hits >= 3)
                Explode();
        }
    }

    // checks if enemy or player was hit by explosion and damages them according
    // to distance away from explosion
    public void ExplodeDamage()
    {
        Collider[] colliders = new Collider[10];
        Vector3 origin = transform.position;
        int collidersCount = Physics.OverlapSphereNonAlloc(origin, radius, colliders, layers);
        //Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        for (int i = 0; i < collidersCount; i++)
        {
           // Debug.Log(colliders[i].name);
            if (colliders[i].CompareTag("Player"))
            {
                float dist = Vector3.Distance(colliders[i].transform.position, transform.position);
                damage = 300 / (dist + 0.1f);
                if (damage >= GameManager.Instance.playerScript.GetCurrentPlayerHealth())
                {
                    if (gameObject.activeInHierarchy == true)
                        gameObject.SetActive(false);
                }
                else
                {
                    //create damage indicator UI
                    DISystem.createIndicator(transform);
                }

                GameManager.Instance.playerScript.TakeDamage((int)damage);
            }
            else if (colliders[i].CompareTag("Hitbox") && colliders[i].GetComponent<CapsuleCollider>() != null){
                float dist = Vector3.Distance(colliders[i].transform.position, transform.position);
                damage = 300 / (dist + 0.1f);
                colliders[i].GetComponent<Health>().TakeDamage((int)damage, transform.position);
            }
        }
    }

    public void Explode()
    {
        if (!isExploded)
        {
            isExploded = true;

            //ObjectPoolManager.Instance.DisableAllInPool(m_FirePool);
            
            for (int i = 0; i < hits; i++)
                m_FirePooled[i].Pool();
            explosion = ObjectPoolManager.Instance.SpawnObject(m_ExplosionPool);
            explosion.transform.position = transform.position;

            audioSource.clip = exploSound;
            audioSource.Play();

            ExplodeDamage();

            if (gameObject.activeInHierarchy == true)
                gameObject.SetActive(false);

            GameManager.Instance.AlertEnemiesInSphere(transform.position, 10f);
        }
    }

    //counter for when to explode the tank
    IEnumerator TimeTillExplode()
    {
        yield return new WaitForSeconds(3);
        Explode();
        m_Mesh.gameObject.SetActive(false);

        if (gameObject.activeInHierarchy == true)
            gameObject.SetActive(false);
    }
}
