
using UnityEngine;

public class ISpawnable : MonoBehaviour
{
    Vector3 m_InitialPosition;
    Quaternion m_InitialRotation;

    public virtual void Spawn()
    {
        m_InitialPosition = transform.position;
        m_InitialRotation = transform.rotation;
    }

    public virtual void Despawn()
    { 

    }

    public virtual void Respawn()
    {
        transform.position = m_InitialPosition;
        transform.rotation = m_InitialRotation;
    }

    public void SetInitialPosition(Vector3 pos)
    {
        m_InitialPosition = pos;
    }

    public void SetInitialRotation(Quaternion quat)
    {
        m_InitialRotation = quat;
    }

    public Vector3 GetInitialPosition()
    {
        return m_InitialPosition;
    }

    public Quaternion GetInitialRotation()
    {
        return m_InitialRotation;
    }
}
