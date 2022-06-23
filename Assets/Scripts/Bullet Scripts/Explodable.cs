using System.Collections;
using UnityEngine;

public class Explodable : MonoBehaviour
{
    [Header("Particle Effects")]
    [SerializeField] PoolableObject fire;
    [SerializeField] PoolableObject explosion;

    // layers that are dectected to deal damage
    [SerializeField] LayerMask layers;

    [SerializeField] LayerMask rayLayers;

    // pool of the fire spray effect
    PoolableObject[] m_FirePooled;

    // barrels mesh
    Transform m_Mesh;

    // name for the particle effects pool
    string m_ExplosionPool;
    string m_FirePool;

    // how many times the barrel was hit
    int hits = 0;

    [SerializeField] float radius;
    float damage;

    bool isExploded = false;
    bool firstHit = false;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    // creates pools
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
            EnemyHitFeedbackManager.Instance.ShowHitFeedback(Color.red);

            StartCoroutine(TimeTillExplode());
            if (!firstHit)
            {
                AudioManager.Instance.Play2dAudioOnce("FireSpray");
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

    /* checks for players or enemys in radius, deals damage based on distance away and objects inbetween */
    public void ExplodeDamage()
    {
        Collider[] colliders = new Collider[10];
        Vector3 origin = transform.position;
        origin.y += 2.5f;
        int collidersCount = Physics.OverlapSphereNonAlloc(origin, radius, colliders, layers);
        
        for (int i = 0; i < collidersCount; i++)
        {
            Debug.Log(colliders[i].gameObject.name);
            Vector3 charPos = new Vector3(colliders[i].transform.position.x, 0, colliders[i].transform.position.z);
            Vector3 explodePos = new Vector3(transform.position.x, 0, transform.position.z);
            float dist = Vector3.Distance(charPos, explodePos);
            damage = 300 - ((dist / 2) * 50);

            if (colliders[i].gameObject.tag == "Player")
            {
                if (dist < 1)
                {
                    ExplodeHitPlayer();
                    break;
                }
            }

            Vector3 target = colliders[i].transform.position;
            target.y += 0.3f;
            if (Physics.Raycast(origin, (target - origin).normalized, out RaycastHit hit, radius, rayLayers))
            {
                Debug.Log(hit.collider.tag + "Raycast hit this: ");
                Debug.DrawLine(origin, origin + ((target - origin).normalized * radius), Color.green, 5f);
                if (hit.collider.CompareTag("Player"))
                {
                    ExplodeHitPlayer();
                }
                else if (hit.collider.CompareTag("Hitbox") && colliders[i].GetComponent<CapsuleCollider>() != null)
                {
                    colliders[i].GetComponent<Health>().TakeDamage((int)damage, transform.position);
                }
            }
            

            //RaycastHit[] results = new RaycastHit[10];
            //Ray ray = new Ray(origin, (target - origin).normalized);
            //int hits = Physics.RaycastNonAlloc(ray, results, radius, rayLayers);
            //Debug.DrawLine(origin, origin + (ray.direction * radius), Color.green, 2f);
            //for (int j = 0; j < hits; j++)
            //{
            //    Debug.Log(results[j].collider.name);
            //    if (results[j].collider.CompareTag("Player"))
            //    {
            //        if (damage >= GameManager.Instance.playerScript.GetCurrentPlayerHealth())
            //        {
            //            if (gameObject.activeInHierarchy == true)
            //                gameObject.SetActive(false);
            //        }
            //        else
            //        {
            //            //create damage indicator UI
            //            DISystem.createIndicator(transform);
            //        }

            //        GameManager.Instance.playerScript.TakeDamage((int)damage);

            //        //add high camera shake
            //        GameManager.Instance.cameraShakeScript.Trauma += 1f;
            //        break;
            //    }
            //    else if (results[j].collider.CompareTag("Hitbox") && colliders[i].GetComponent<CapsuleCollider>() != null)
            //    {
            //        colliders[i].GetComponent<Health>().TakeDamage((int)damage, transform.position);
            //        break;
            //    }
            //    else if (results[j].collider.CompareTag("EnvironmentWood")) { damage *= 0.6f; }
            //    else if (results[j].collider.CompareTag("EnvironmentMetal")) { damage *= 0.3f; }
            //    else if (results[j].collider.CompareTag("EnvironmentConcrete")) { damage = 0; }
            //}
        }

        //whether or not it was close enough to the player, at least add a small amount of camera shake
        GameManager.Instance.cameraShakeScript.Trauma += 0.3f;
    }

    // called when the bullet hits the explodable
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

            AudioManager.Instance.Play2dAudioOnce("FireExplosion");

            ExplodeDamage();

            if (gameObject.activeInHierarchy == true)
                gameObject.SetActive(false);

            GameManager.Instance.AlertEnemiesInSphere(transform.position, 10f);
        }
    }


    void ExplodeHitPlayer()
    {
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

        //add high camera shake
        GameManager.Instance.cameraShakeScript.Trauma += 1f;
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
