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
        Collider[] colliders = new Collider[10];
        Vector3 origin = transform.position;
        origin.y += 1f;
        int collidersCount = Physics.OverlapSphereNonAlloc(origin, scanSphereRadius, colliders, GetLayerMask());
        Debug.Log("scan colliders: " + collidersCount);
        if (collidersCount > 0)
        {
            for (int i = 0; i < collidersCount; i++)
            {
                Debug.Log("Scanned: " + colliders[i].tag);
                if (colliders[i].tag == "Hitbox")
                {
                    if (scanThroughWalls == false){
                        RaycastHit hitInfo;
                        if (Physics.Raycast(origin, (colliders[i].transform.position - origin).normalized, 
                            out hitInfo))
                        {
                            if (hitInfo.collider.CompareTag("Hitbox"))
                            {
                                colliders[i].GetComponent<MiniMapScanable>().Show();
                            }
                        }
                    }
                    else
                    {
                        colliders[i].GetComponent<MiniMapScanable>().Show();
                    }
                }
            }
        }

        //Debug.Log(name + " just exploded!");
        Invoke("Pool", GetPoolTimeAfterExplosion());
    }

    /*
    * Adds a force in the forceDirection with a scale of forceMultiplier
    */
    public override void OnThrowThrowable(Vector3 forceDirection, float forceMultiplier = 1f)
    {
        Debug.Log("Throw" + name);
        GetRigidbody().AddForce(forceDirection * forceMultiplier, ForceMode.Impulse);
        StartCoroutine(OnThrowableExplode());
    }
}
