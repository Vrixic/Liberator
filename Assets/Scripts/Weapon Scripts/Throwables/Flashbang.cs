using System.Collections;
using UnityEngine;

public class Flashbang : BaseThrowables
{
    [SerializeField] float flashTimer;

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
        Collider[] colliders = new Collider[14];
        Vector3 origin = transform.position;
        origin.y += 2f;
        int collidersCount = Physics.OverlapSphereNonAlloc(origin, sphereRadius, colliders, GetLayerMask());

        //if the player is close to the flashbang(not necessarily effected) apply camera shake
        if ((GameManager.Instance.playerTransform.position - transform.position).sqrMagnitude < sphereRadius * sphereRadius * 1.5f)
            GameManager.Instance.cameraShakeScript.Trauma += 0.65f;

        float raycastDistance = sphereRadius * 2f;

        if (collidersCount > 0)
            EnemyHitFeedbackManager.Instance.ShowHitFeedback(Color.white);

        for (int i = 0; i < collidersCount; i++)
        {
            Vector3 target = colliders[i].transform.position;

            //give the flashbang a better chance of hitting enemies by raycasting from a higher position.
            if(colliders[i].CompareTag("Hitbox"))
                target.y += 1.1f; // so it doesn't target the ground

            //Debug.DrawLine(origin, origin + (target - origin).normalized * raycastDistance, Color.green, 2f);
            if (Physics.Raycast(origin, (target - origin).normalized, out RaycastHit hitInfo, raycastDistance))
            {
                if (hitInfo.collider.CompareTag("Player"))
                {
                    GameManager.Instance.playerScript.FlashPlayer();
                }
                else if (hitInfo.collider.CompareTag("Hitbox"))
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
