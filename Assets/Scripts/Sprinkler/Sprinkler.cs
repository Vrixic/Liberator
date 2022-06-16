using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprinkler : MonoBehaviour
{
    public float stunTime = 5.0f;
    BoxCollider stunTrigger;

    [SerializeField] ParticleSystem spray; 

    bool isUsed;
    // Start is called before the first frame update
    void Start()
    {
        stunTrigger = gameObject.GetComponentInChildren<BoxCollider>();
        stunTrigger.enabled = false;
        isUsed = false;
    }


    public void Sprinkle()
    {
        stunTrigger.enabled = true;
        if (!isUsed) { 
            spray = Instantiate(spray, transform.position - new Vector3(0,2,0), Quaternion.identity);
            isUsed = true;
        }

        Invoke("DeactivateSprinkler", 5f);
    }

    void DeactivateSprinkler()
    {
        spray.gameObject.SetActive(false);
        stunTrigger.enabled = false;
        gameObject.GetComponentInChildren<FlashingLight>().Disable();
    }

}