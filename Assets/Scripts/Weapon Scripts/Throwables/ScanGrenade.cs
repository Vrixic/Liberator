using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanGrenade : BaseThrowables
{
    [SerializeField] bool scanThroughWalls = true;

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

        AudioManager.Instance.PlayAudioAtLocation(transform.position, "ScanGrenade");
        PlayExplodeSFX();

        // Do Physics.OverlapSphereNonAlloc here
        Collider[] colliders = new Collider[20];
        Vector3 origin = transform.position;
        origin.y += 1f;
        int collidersCount = Physics.OverlapSphereNonAlloc(origin, sphereRadius, colliders, GetLayerMask());
        //Debug.Log("scan colliders: " + collidersCount);

        //if the player is close to the flashbang(not necessarily effected) apply camera shake
        if ((GameManager.Instance.playerTransform.position - transform.position).sqrMagnitude < sphereRadius * sphereRadius * 1.5f)
            GameManager.Instance.cameraShakeScript.Trauma += 0.4f;

        float raycastDistance = sphereRadius * 2f;

        for (int i = 0; i < collidersCount; i++)
        {

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
                        hitInfo.collider.GetComponent<MiniMapScanable>().Show(equipmentTimer);
                    }                        
                }
            }
            else
            {
                if (colliders[i].GetComponent<CapsuleCollider>() != null)
                {
                    colliders[i].GetComponent<MiniMapScanable>().Show(equipmentTimer);
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
