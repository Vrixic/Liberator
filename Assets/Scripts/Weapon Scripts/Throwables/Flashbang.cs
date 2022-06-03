using System.Collections;
using UnityEngine;

public class Flashbang : BaseThrowables
{

    [SerializeField] float flashSphereRadius = 50f;

    /*
    * Called when flashbang explodes
    *   - Returns after the explode timer                                                         
    *   - Plays both the explosion audio and the particle system
    *   - Does a sphere cast that returns all the colliders in the sphere, max 10, then checks if any of them was player or enemy, if so, flash them
    *   - Pools the object after the pool timer
    */
    public override IEnumerator OnThrowableExplode()
    {
        yield return new WaitForSeconds(GetExplodeTimer());

        PlayExplodeAudio();
        PlayExplodeSFX();

        // Do Physics.OverlapSphereNonAlloc here
        Collider[] colliders = new Collider[10];
        Vector3 origin = transform.position;
        origin.y += 1f;
        int collidersCount = Physics.OverlapSphereNonAlloc(origin, flashSphereRadius, colliders, GetLayerMask());
        
        for (int i = 0; i < collidersCount; i++)
        {
            Debug.Log("Flash: " + colliders[i].tag);
            if (colliders[i].CompareTag("Player"))
            {
                GameManager.Instance.playerScript.FlashPlayer();
            }
            else
            {
                colliders[i].GetComponentInParent<AIAgent>().stateMachine.ChangeState(AIStateID.Flashed);
            }
        }

        Invoke("Pool", GetPoolTimeAfterExplosion());
    }

    /*
    * Adds a force in the forceDirection with a scale of forceMultiplier
    */
    public override void OnThrowThrowable(Vector3 forceDirection, float forceMultiplier = 1f)
    {
        transform.Translate(.5f, 0, 0);
        GetRigidbody().AddForce(forceDirection * forceMultiplier, ForceMode.Impulse);
        StartCoroutine(OnThrowableExplode());
    }
}
