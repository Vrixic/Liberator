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

        float minX = centerPoint.x - coneRadius.x;
        float maxX = centerPoint.x + coneRadius.x;

        float minY = centerPoint.y - coneRadius.y;
        float maxY = centerPoint.y + coneRadius.y;

        for (int i = 0; i < pellets; i++)
        {
            Bullet bullet = ObjectPoolManager.Instance.SpawnObject(GetBulletPool()) as Bullet;

            targetPoints = centerPoint;

            targetPoints.x = Random.Range(minX, maxX);
            targetPoints.y = Random.Range(minY, maxY);

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
