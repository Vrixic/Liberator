using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public PoolableObject poolableAudioSource;

    public AudioSource musicAudioSource;
    public AudioSource buttonAudioSource;

    string POOLNAME = "AudioSourcePool";
    PoolableObject poolable;
    AudioSource audioSource;
    Sound sound;
    bool bPoolReady = false;

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
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {
        POOLNAME = ObjectPoolManager.Instance.CreateObjectPool(poolableAudioSource, 50);
        foreach (SoundEffect sound in sfxSounds)
        {
            audioSoundsAudioClipDictionary.Add(sound.objectTag, sound.sound);
        }
        bPoolReady = true;
        PlayAudioAtLocation(Vector3.zero, "MenuMusic");
    }

    private void Update()
    {
        musicAudioSource.volume = (PlayerPrefManager.Instance.musicVolume / 2000) ;
    }

    public void PlayAudioAtLocation(Vector3 location, string objectTag, float volM = 1f, bool isLandingSound = false)
    {
        if (!bPoolReady) return;

        poolable = ObjectPoolManager.Instance.SpawnObject(POOLNAME);
        if (audioSoundsAudioClipDictionary.ContainsKey(objectTag))
        {
            sound = audioSoundsAudioClipDictionary[objectTag];
        }
        else
        {
            sound = audioSoundsAudioClipDictionary[sfxSounds[0].objectTag];
        }

        if (sound.audioType == AudioType.sfx) 
        {
            audioSource = poolable.GetComponent<AudioSource>();
            audioSource.transform.position = location;
            audioSource.volume = (PlayerPrefManager.Instance.sfxVolume/100) * sound.volMultiplier;
            audioSource.PlayOneShot(GetAudioClip(objectTag));

        }
        else if (sound.audioType == AudioType.ui)
        {
            musicAudioSource.volume = (PlayerPrefManager.Instance.musicVolume / 2000);
            musicAudioSource.clip = GetAudioClip(objectTag);
            musicAudioSource.Play();
        }
        else if (sound.audioType == AudioType.footstep)
        {
            audioSource = poolable.GetComponent<AudioSource>();
            audioSource.transform.position = location;
            audioSource.volume = (PlayerPrefManager.Instance.sfxVolume / 100) * sound.volMultiplier;
            if (isLandingSound) { audioSource.volume *= volM; }
            audioSource.PlayOneShot(GetAudioClip(objectTag));
        }
    }

    public void Play2dAudioOnce(string objectTag)
    {
        sound = audioSoundsAudioClipDictionary[objectTag];
        buttonAudioSource.volume = (PlayerPrefManager.Instance.sfxVolume / 100) * sound.volMultiplier;
        buttonAudioSource.PlayOneShot(GetAudioClip(objectTag));
    }

    public AudioClip GetAudioClip(string objectTag)
    {
        if (audioSoundsAudioClipDictionary.ContainsKey(objectTag))
        {
            return audioSoundsAudioClipDictionary[objectTag].audio[Random.Range(0, audioSoundsAudioClipDictionary[objectTag].audio.Length)];
        }
        else
        {
            return audioSoundsAudioClipDictionary[sfxSounds[0].objectTag].audio[0];
        }
    }

    public void PlaySoundAtVolume(float vol, string objectTag)
    {
        buttonAudioSource.volume = vol/100;
        buttonAudioSource.PlayOneShot(GetAudioClip(objectTag));
    }

    public void StopMusic()
    {
        musicAudioSource.Stop();
    }

    public void PauseMusic()
    {
        musicAudioSource.Pause();
    }

    public void ResumeMusic()
    {
        musicAudioSource.UnPause();
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
        public AudioClip[] audio;
        public float volMultiplier = 1f;
    }

    public enum AudioType
    {
        sfx,
        ui,
        footstep,
        other
    }
}