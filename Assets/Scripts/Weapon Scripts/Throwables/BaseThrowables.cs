using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseThrowables : PoolableObject
{
    /* time it takes before the throwable expodes */
    [SerializeField] [Tooltip("time it takes before the throwable expodes")] float throwableExplodeTimer = 1.0f;

    /* particle system to be played when the throwable explodes */
    [SerializeField] [Tooltip("particle system to be played when the throwable explodes")] PoolableObject explodeParticleSystem;

    /* the time in seconds it should wait before pooling this throwable back to the object pool */
    [SerializeField] [Tooltip("the time in seconds it should wait before pooling this throwable back to the object pool")] float poolTimeAfterExplosion = 1.0f;

    [SerializeField] [Tooltip("On explosion, only these layers will be queried")] LayerMask queryLayers;

    /* the audio source used to play audio for this throwable */
    AudioSource m_AudioSource;

    /* rigidbody for this throwable */
    Rigidbody m_RigidBody;

    string m_ExplosionSFXPool;

    public BaseThrowables() { }

    /*
    * sets both audio source and rigidbody of this throwable
    */
    public override void OnStart()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_RigidBody = GetComponent<Rigidbody>();

        m_ExplosionSFXPool = ObjectPoolManager.Instance.CreateObjectPool(explodeParticleSystem, 1);
    }

    /*
    * called when the throwable explodes
    */
    public virtual IEnumerator OnThrowableExplode()
    {
        yield return new WaitForSeconds(throwableExplodeTimer);

        PlayExplodeSFX();

        Debug.Log(name + " just exploded!");

        Invoke("Pool", poolTimeAfterExplosion);
    }

    /*
    * Called when the player throws this throwable
    */
    public virtual void OnThrowThrowable(Vector3 forceDirection, float forceMultiplier = 1f)
    {
        Debug.Log("Throw" + name);
        StartCoroutine(OnThrowableExplode());
    }

    /*
    * plays the explosion particle effect
    */
    protected void PlayExplodeSFX()
    {
        PoolableObject poolObject = ObjectPoolManager.Instance.SpawnObject(m_ExplosionSFXPool);
        poolObject.transform.position = transform.position;
        poolObject.transform.rotation = transform.rotation;
    }

    /*
    * returns the explode time for this throwable
    */
    public float GetExplodeTimer()
    {
        return throwableExplodeTimer;
    }

    /*
    * returns the pool time after explosion
    */
    public float GetPoolTimeAfterExplosion()
    {
        return poolTimeAfterExplosion;
    }

    /*
    * returns the rigidbody
    */
    protected Rigidbody GetRigidbody()
    {
        return m_RigidBody;
    }

    public void SetLayerMask(LayerMask mask)
    {
        queryLayers = mask;
    }

    protected LayerMask GetLayerMask()
    {
        return queryLayers;
    }
}
