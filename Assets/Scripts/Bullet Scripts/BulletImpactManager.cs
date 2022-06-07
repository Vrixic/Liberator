using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BulletImpactManager : MonoBehaviour
{
    /* size of the each particle systems object pool */
    [SerializeField] private int particleSystemBuffer = 5;

    /* list of bullet impacts */
    public List<Impact> bulletImpacts = new List<Impact>();

    /* stores the object pool related to a string, second string is the pool name */
    public Dictionary<string, string> bulletImpactDictionary = new Dictionary<string, string>();

    public Dictionary<string, AudioClip> bulletImpactAudioClipDictionary = new Dictionary<string, AudioClip>();

    AudioSource m_AudioSource;

    public float volumeMultiplier = 1f;

    /* instance of this object, singleton pattern */
    private static BulletImpactManager m_Instance;
    public static BulletImpactManager Instance
    {
        get
        {
            return m_Instance;
        }
        private set
        {
            m_Instance = value;
        }
    }

    /*
     * copys all of the bullet impacts from list to the dictionary
     */

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple BulletImpactManagers! Destroying the newest one: " + this.name);
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        foreach (Impact impact in bulletImpacts)
        {
            bulletImpactDictionary.Add(impact.objectTag, ObjectPoolManager.Instance.CreateObjectPool(impact.collisionParticleSystem, particleSystemBuffer));
            bulletImpactAudioClipDictionary.Add(impact.objectTag, impact.impactAudio);
        }

        m_AudioSource = GetComponent<AudioSource>();
    }

    /*
     * Spawns a impact at position, with the forward direction, impact is found by @Param: objectTag
     */
    public void SpawnBulletImpact(Vector3 position, Vector3 forward, string objectTag)
    {
        if (bulletImpactDictionary.ContainsKey(objectTag))
        {
            UpdateSpawnBulletImpact(position, forward, ObjectPoolManager.Instance.SpawnObject(bulletImpactDictionary[objectTag]));
        }
        else
        {
            UpdateSpawnBulletImpact(position, forward, ObjectPoolManager.Instance.SpawnObject(bulletImpactDictionary[bulletImpacts[0].objectTag]));
        }
    }

    public void PlayAudioAtLocation(Vector3 location, string objectTag)
    {
        SetAudioVolume(PlayerPrefManager.Instance.sfxVolume/100);

        m_AudioSource.transform.position = location;
        m_AudioSource.PlayOneShot(GetAudioClipForImpactFromTag(objectTag));
    }

    public void SetAudioVolume(float val)
    {
        m_AudioSource.volume = val * volumeMultiplier;
    }

    public AudioClip GetAudioClipForImpactFromTag(string objectTag)
    {
        if (bulletImpactDictionary.ContainsKey(objectTag))
        {
            return bulletImpactAudioClipDictionary[objectTag];
        }
        else
        {
            return bulletImpactAudioClipDictionary[bulletImpacts[0].objectTag];
        }
    }

    /*
     * Updates the particle systems forward and position vectors
     */
    private void UpdateSpawnBulletImpact(Vector3 position, Vector3 forward, PoolableObject particleSystem)
    {
        particleSystem.transform.position = position;
        particleSystem.transform.forward = forward;
    }


    /*
     * Stores the tag and the particle system associated with the tag
     */
    [System.Serializable]
    public class Impact
    {
        public string objectTag;
        public PoolableObject collisionParticleSystem;
        public AudioClip impactAudio;
    }
}