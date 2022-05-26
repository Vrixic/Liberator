using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] GameObject m_Object;
    private ISpawnable spawnable;

    [SerializeField] bool isPlayer;

    private void Start()
    {
        spawnable = m_Object.GetComponentInChildren<ISpawnable>();

        if (!isPlayer)
        {
            spawnable.SetInitialPosition(transform.position);
            spawnable.SetInitialRotation(transform.rotation);
        }
    }

    public virtual void Respawn()
    {
        //if (isPlayer) return;

        m_Object.transform.localPosition = Vector3.zero;
        m_Object.transform.rotation = Quaternion.identity;

        spawnable.Respawn();
    }

    public bool IsPlayerSpawn()
    {
        return isPlayer;
    }
}
