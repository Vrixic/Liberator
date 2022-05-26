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

        m_Object.transform.localPosition = spawnable.GetInitialPosition();
        m_Object.transform.rotation = spawnable.GetInitialRotation();

        spawnable.Respawn();
    }

    public bool IsPlayerSpawn()
    {
        return isPlayer;
    }
}
