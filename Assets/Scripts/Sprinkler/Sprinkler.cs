using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprinkler : MonoBehaviour
{
    public float stunTime = 5.0f;
    BoxCollider stunTrigger;
    Sprinkler sprinkle;

    [SerializeField] ParticleSystem spray;

    bool isUsed;
    // Start is called before the first frame update
    void Start()
    {
        stunTrigger = gameObject.GetComponentInChildren<BoxCollider>();
        stunTrigger.enabled = false;
        sprinkle = gameObject.GetComponent<Sprinkler>();
        isUsed = false;
    }

    public void Sprinkle()
    {
       Debug.Log("Sprinkler Shot");
       stunTrigger.enabled = true;
       if (isUsed == false) { Instantiate(spray, transform.position, Quaternion.identity); }
       isUsed = true;
       
       StartCoroutine(DeactivateSprinkler());
    }

    IEnumerator DeactivateSprinkler()
    {
        yield return new WaitForSeconds(5.0f);
        sprinkle.enabled = false;
        stunTrigger.enabled = false;
    }

}
