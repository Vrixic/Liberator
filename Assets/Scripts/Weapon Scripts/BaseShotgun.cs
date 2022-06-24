using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShotgun : BaseGun
{
    [SerializeField] Vector2 bulletConeRadius;
    [SerializeField] Vector2 aimConeRadius;
    Vector2 coneRadius;
    [SerializeField] int pellets = 9;


    public override void ShootBullet()
    {
        if (bIsAiming)
        {
            coneRadius = aimConeRadius;
        }
        else
        {
            coneRadius = bulletConeRadius;
        }

        Vector3 centerPoint = raycastOrigin.position + GameManager.Instance.mainCamera.transform.forward * GetBulletRange();
        Vector3 targetPoints;

        for (int i = 0; i < pellets; i++)
        {
            Bullet bullet = ObjectPoolManager.Instance.SpawnObject(GetBulletPool()) as Bullet;
            targetPoints = centerPoint;

            targetPoints += GameManager.Instance.mainCamera.transform.right * Random.Range(-coneRadius.x, coneRadius.x);
            targetPoints += GameManager.Instance.mainCamera.transform.up * Random.Range(-coneRadius.y, coneRadius.y);

            RaycastHit hitInfo;
            if (Physics.Linecast(raycastOrigin.position, targetPoints, out hitInfo, raycastLayers))
            {
                OnRayCastHit(bullet, hitInfo);
            }
        }

        //add significant camera shake to make it feel powerful
        GameManager.Instance.cameraShakeScript.Trauma = 0.7f;
    }
}
