using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShotgun : BaseGun
{
    [SerializeField] Vector2 bulletConeRadius;
    [SerializeField] int pellets = 8;

    public override void ShootBullet()
    {
        Vector3 centerPoint = raycastOrigin.position + GameManager.Instance.mainCamera.transform.forward * GetBulletRange();
        Vector3 targetPoints;

        float minX = centerPoint.x - bulletConeRadius.x;
        float maxX = centerPoint.x + bulletConeRadius.x;

        float minY = centerPoint.y - bulletConeRadius.y;
        float maxY = centerPoint.y + bulletConeRadius.y;

        for (int i = 0; i < pellets; i++)
        {
            Bullet bullet = ObjectPoolManager.SpawnObject(GetBulletPool()) as Bullet;

            targetPoints = centerPoint;

            targetPoints.x = Random.Range(minX, maxX);
            targetPoints.y = Random.Range(minY, maxY);

            RaycastHit hitInfo;
            if (Physics.Linecast(raycastOrigin.position, targetPoints, out hitInfo, raycastLayers))
            {
                OnRayCastHit(bullet, hitInfo);
            }
        }
    }
}
