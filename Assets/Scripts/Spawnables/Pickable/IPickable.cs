using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class IPickable : MonoBehaviour
{
    public virtual void Start()
    {
        GetComponent<SphereCollider>().isTrigger = true;
    }

    public virtual void OnPickup(GameObject picker) { }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            OnPickup(other.gameObject);
    }
}
