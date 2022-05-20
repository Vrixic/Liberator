using System.Collections;
using UnityEngine;

public class Explodable : ISpawnable
{
    public PoolableObject fire;
    public PoolableObject explosion;

    string m_ExplosionPool;
    string m_FirePool;

    int hits = 0;
    int counter = 0;

    private void Start()
    {
        m_ExplosionPool = ObjectPoolManager.CreateObjectPool(explosion, 1);
        m_FirePool = ObjectPoolManager.CreateObjectPool(fire, 3); ;
    }

    public void Explode(Vector3 pos, Vector3 forward)
    {
        StartCoroutine(TimeTillExplode());

        fire = ObjectPoolManager.SpawnObject(m_FirePool);
        fire.transform.position = pos;
        fire.transform.forward = (Vector3.up - forward).normalized;

        hits++;
    }

    private void Update()
    {
        if (counter == 3 || hits == 3)
        {
            explosion = ObjectPoolManager.SpawnObject(m_ExplosionPool);
            explosion.transform.position = transform.position;

            StopCoroutine(TimeTillExplode());
            counter = 0;

            Despawn();
        }
    }

    public override void Despawn()
    {
        base.Despawn();

        gameObject.SetActive(false);
    }

    public override void Respawn()
    {
        base.Respawn();

        hits = 0;
        counter = 0;
    }

    IEnumerator TimeTillExplode()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            counter++;
        }
    }
}
