using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodable : MonoBehaviour
{
    public GameObject fire;
    public GameObject explosion;

    int hits = 0;
    int counter = 0;

    public void Explode(Vector3 pos)
    {
        StartCoroutine(TimeTillExplode());
        fire = Instantiate(fire, transform.position, Quaternion.identity);
        fire.transform.forward = pos.normalized;
        hits++;
    }

    private void Update()
    {
        if (counter == 3 || hits == 3)
        {
            Instantiate(explosion, transform.position, transform.rotation);
            StopCoroutine(TimeTillExplode());
            counter = 0;
            Destroy(gameObject);
        }
    }

    IEnumerator TimeTillExplode()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            counter++;
        }
    }
}
