using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class Bullet : PoolableObject
{
    /* time delay before bullet is pooled after collision */
    [Tooltip("time delay before bullet is pooled after collision, -2 = instant pool")]public float destroyTimeAfterCollision = 1f;

    /* speed of bullet */
    public float moveSpeed = 25f;

    /* spawned position of bullet */
    Vector3 m_StartPosition;

    /* range the bullet can go to before it gets pooled, squared range */
    float m_BulletRange = 0f;

    TrailRenderer m_TrailRenderer;

    public override void OnStart()
    {
        base.OnStart();

        m_TrailRenderer = GetComponent<TrailRenderer>();
    }


    public override void OnEnable()
    {
        m_IsAlreadyPooled = false;

        if (autoPoolTime > 0)
            Invoke("Pool", autoPoolTime);
    }

    /*
     * Raycasts casted to check if bullet collided with an object, if so, OnRayCastHit, is called
     * If bullet exceeds teh bullet range, it gets pooled
     */
    private void FixedUpdate()
    {
        float nextPositionMultiplier = moveSpeed * Time.deltaTime;
        transform.position = transform.position + (transform.forward * nextPositionMultiplier);

        if ((m_StartPosition - transform.position).sqrMagnitude >= m_BulletRange)
            ReturnBulletToPool();
    }

    /*
     * Bullet collided with an object
     * Spawns a impact particle based on what was hit
     * Invokes disable method for bullet to get pooled
     */
    void OnRayCastHit(RaycastHit hit)
    {
        if (hit.collider.tag == "Hitbox")
        {
            if (hit.collider.gameObject.GetComponent<SphereCollider>())
            {
                AudioManager.Instance.PlayAudioAtLocation(GameManager.Instance.playerScript.transform.position, "EnemyHeadshot");
            }
            else
            {
                AudioManager.Instance.PlayAudioAtLocation(GameManager.Instance.playerScript.transform.position, "BulletHitEnemy");
            }
            
        }
        else if (hit.collider.tag == "Explodable")
        {
            Explodable e = hit.collider.GetComponent<Explodable>();
            e.ExplodableIsHit(hit.point, hit.normal);
        }
        else if (hit.collider.tag == "Sprinkler")
        {
            Sprinkler s = hit.collider.GetComponent<Sprinkler>();
            s.Sprinkle();
        }
        

        BulletImpactManager.Instance.SpawnBulletImpact(hit.point, hit.normal, hit.collider.tag);
    }

    public void Spawn(Vector3 position, Vector3 forward, float bulletRange)
    {
        m_StartPosition = position;
        transform.position = position;
        transform.forward = forward;

        m_TrailRenderer.SetPosition(0, position);

        m_BulletRange = bulletRange;
    }

    public void Spawn(Vector3 position, Vector3 forward, RaycastHit hit, float audioVolume = 1f)
    {
        m_StartPosition = position;
        transform.position = position;
        transform.forward = forward;

        m_TrailRenderer.SetPosition(0, position);

        m_BulletRange = (hit.point - position).sqrMagnitude;

        OnRayCastHit(hit);

        BulletImpactManager.Instance.PlayAudioAtLocation(hit.point, hit.collider.tag);
    }

    /*
     * Returns bullet to it pool
     */
    private void ReturnBulletToPool()
    {
        Pool();
    }
}