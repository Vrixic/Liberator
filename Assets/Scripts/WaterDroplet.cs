using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDroplet : MonoBehaviour
{
    private ParticleSystem m_WaterDropPS;

    private void Start()
    {
        m_WaterDropPS = GetComponent<ParticleSystem>();
        StartCoroutine(EmitWaterDrops());
    }

    private void OnParticleCollision(GameObject other)
    {
        AudioManager.Instance.PlayAudioAtLocation(transform.position, "WaterDrip");
    }

    IEnumerator EmitWaterDrops()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f));
            m_WaterDropPS.Emit(1);
        }
    }
}