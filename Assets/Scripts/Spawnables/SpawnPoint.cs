using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Vector3 offset;

    private ISpawnable spawnable;

    
    private void Start()
    {
        prefab = Instantiate(prefab, transform.position + offset, transform.rotation);
        gameObject.SetActive(false);

        spawnable = prefab.GetComponentInChildren<ISpawnable>();
    }
}
