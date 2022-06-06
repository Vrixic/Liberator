using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public PoolableObject pAudioSource;

    /* list of sounds */
    public List<SoundEffect> sfxSounds = new List<SoundEffect>();

    public Dictionary<string, string> audioSourceDictionary = new Dictionary<string, string>();

    public Dictionary<string, Sound> audioSoundsAudioClipDictionary = new Dictionary<string, Sound>();

    /* instance of this object, singleton pattern */
    private static AudioManager m_Instance;
    public static AudioManager Instance
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
     * copys all of the sounds from list to the dictionary
     */

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {
        foreach (SoundEffect sound in sfxSounds)
        {
            audioSourceDictionary.Add(sound.objectTag, ObjectPoolManager.Instance.CreateObjectPool(pAudioSource, 1));
            audioSoundsAudioClipDictionary.Add(sound.objectTag, sound.sound);
        }
    }

    public void PlayAudioAtLocation(Vector3 location, string objectTag, bool isPlayer = false)
    {
        PoolableObject poolable;
        AudioSource audioSource;
        Sound sound;
        if (audioSoundsAudioClipDictionary.ContainsKey(objectTag))
        {
            poolable = ObjectPoolManager.Instance.SpawnObject(audioSourceDictionary[objectTag]);
            sound = audioSoundsAudioClipDictionary[objectTag];
        }
        else
        {
            poolable = ObjectPoolManager.Instance.SpawnObject(audioSourceDictionary[sfxSounds[0].objectTag]);
            sound = audioSoundsAudioClipDictionary[sfxSounds[0].objectTag];
        }
        if (isPlayer)
        {
            poolable.transform.position = GameManager.Instance.playerScript.transform.position;
        }
        else
        {
            poolable.transform.position = location;
        }

        audioSource = poolable.GetComponent<AudioSource>();
        
        if (sound.audioType == AudioType.sfx) {
            audioSource.volume = (PlayerPrefManager.Instance.sfxVolume/100) * sound.volMultiplier; }
        else if (sound.audioType == AudioType.ui) {
            audioSource.volume = (PlayerPrefManager.Instance.musicVolume/100) * sound.volMultiplier; }
        else { audioSource.volume = 1f; }

        audioSource.PlayOneShot(GetAudioClip(objectTag));
    }

    public AudioClip GetAudioClip(string objectTag)
    {
        if (audioSoundsAudioClipDictionary.ContainsKey(objectTag))
        {
            return audioSoundsAudioClipDictionary[objectTag].audio;
        }
        else
        {
            return audioSoundsAudioClipDictionary[sfxSounds[0].objectTag].audio;
        }
    }

    /*
     * Stores the tag and the particle system associated with the tag
     */
    [System.Serializable]
    public class SoundEffect
    {
        public Sound sound;
        public string objectTag;
    }

    [System.Serializable]
    public class Sound
    {
        public AudioType audioType;
        public AudioClip audio;
        public float volMultiplier = 1f;
    }

    public enum AudioType
    {
        sfx,
        ui
    }
}