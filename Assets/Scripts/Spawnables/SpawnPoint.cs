using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] GameObject m_Object;
    private ISpawnable spawnable;

    private void Start()
    {
        spawnable = m_Object.GetComponentInChildren<ISpawnable>();

        spawnable.SetInitialPosition(transform.position);
        spawnable.SetInitialRotation(transform.rotation);
    }

    public virtual void Respawn()
    {
        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = Quaternion.identity;

        spawnable.Respawn();
    }
}
