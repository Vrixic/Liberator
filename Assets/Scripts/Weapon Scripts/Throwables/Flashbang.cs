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

        AudioManager.Instance.PlayAudioAtLocation(transform.position, "FlashBang");
        PlayExplodeSFX();

        // Do Physics.OverlapSphereNonAlloc here
        Collider[] colliders = new Collider[10];
        Vector3 origin = transform.position;
        origin.y += 2f;
        int collidersCount = Physics.OverlapSphereNonAlloc(origin, flashSphereRadius, colliders, GetLayerMask());

        float raycastDistance = flashSphereRadius * 2f;

        for (int i = 0; i < collidersCount; i++)
        {
            Vector3 target = colliders[i].transform.position;
            target.y += 1.1f; // so it doesn't target the ground

            //Debug.DrawLine(origin, origin + (target - origin).normalized * raycastDistance, Color.green, 2f);
            RaycastHit hitInfo;
            if (Physics.Raycast(origin, (target - origin).normalized, out hitInfo, raycastDistance))
            {
                if (hitInfo.collider.tag == "Player")
                {
                    GameManager.Instance.playerScript.FlashPlayer();
                }
                else if (hitInfo.collider.tag == "Hitbox")
                {
                    colliders[i].GetComponentInParent<AIAgent>().stateMachine.ChangeState(AIStateID.Flashed);
                }
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
