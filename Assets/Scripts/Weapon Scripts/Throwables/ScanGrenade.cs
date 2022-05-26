using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanGrenade : BaseThrowables
{
    [SerializeField] float scanSphereRadius = 5f;
    [SerializeField] bool scanThroughWalls = false;

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
        Collider[] colliders = new Collider[20];
        Vector3 origin = transform.position;
        origin.y += 1f;
        int collidersCount = Physics.OverlapSphereNonAlloc(origin, scanSphereRadius, colliders, GetLayerMask());
        //Debug.Log("scan colliders: " + collidersCount);

        float raycastDistance = scanSphereRadius * 2f;

        for (int i = 0; i < collidersCount; i++)
        {
            Debug.Log("Scanned: " + colliders[i].tag);

            if (scanThroughWalls == false)
            {
                Vector3 target = colliders[i].transform.position;
                target.y += 0.25f; // so it doesn't target the ground

                //Debug.DrawLine(origin, origin + (target - origin).normalized * raycastDistance, Color.green, 2f);
                RaycastHit hitInfo;
                if (Physics.Raycast(origin, (target - origin).normalized, out hitInfo, raycastDistance))
                {
                    if(hitInfo.collider.tag == "Hitbox")
                    {
                        Debug.Log("scannin: " + colliders[i].tag);
                        hitInfo.collider.GetComponent<MiniMapScanable>().Show();
                    }                        
                }
            }
            else
            {
                colliders[i].GetComponent<MiniMapScanable>().Show();
            }
        }

        Invoke("Pool", GetPoolTimeAfterExplosion());
    }

    /*
    * Adds a force in the forceDirection with a scale of forceMultiplier
    */
    public override void OnThrowThrowable(Vector3 forceDirection, float forceMultiplier = 1f)
    {
        transform.Translate(.6f, 0, 0); 
        GetRigidbody().AddForce(forceDirection * forceMultiplier, ForceMode.Impulse);
        StartCoroutine(OnThrowableExplode());
    }
}
