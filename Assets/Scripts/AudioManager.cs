using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    //audio source for moving audio, gunshots exc.
    public PoolableObject poolableAudioSource;

    //audio source for music
    public AudioSource musicAudioSource;
    // audio source for menu buttons, (2d audio source)
    public AudioSource buttonAudioSource;

    // name of the pool for the audio source pool
    string POOLNAME = "AudioSourcePool";
    PoolableObject poolable;

    // vars for play audio at location method
    AudioSource audioSource;
    Sound sound;

    // bool used to make sure audio doesnt try to play before the dictionary is filled
    bool bPoolReady = false;

    /* list of sounds */
    public List<SoundEffect> sfxSounds = new List<SoundEffect>();

    // dictionary for the audio clips
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

    // fills the dictionary with the sounds from the list
    private void Start()
    {
        POOLNAME = ObjectPoolManager.Instance.CreateObjectPool(poolableAudioSource, 50);
        foreach (SoundEffect sound in sfxSounds)
        {
            audioSoundsAudioClipDictionary.Add(sound.objectTag, sound.sound);
        }
        bPoolReady = true;
        //starts menu music
        PlayAudioAtLocation(Vector3.zero, "MenuMusic");
    }

    private void Update()
    {
        musicAudioSource.volume = (PlayerPrefManager.Instance.musicVolume / 2000) ;
    }

    // used to play 3d audio at a specific location
    public void PlayAudioAtLocation(Vector3 location, string objectTag, float volM = 1f, bool isLandingSound = false)
    {
        if (!bPoolReady) return;

        if (audioSoundsAudioClipDictionary.ContainsKey(objectTag))
        {
            sound = audioSoundsAudioClipDictionary[objectTag];
        }
        else
        {
            sound = audioSoundsAudioClipDictionary[sfxSounds[0].objectTag];
        }

        if (sound.audioType == AudioType.sfxPlayer)
        {
            AudioSource aS = GameManager.Instance.playerScript.m_PlayerAudioSrc;
            aS.volume = (PlayerPrefManager.Instance.sfxVolume / 100) * sound.volMultiplier;
            if (isLandingSound) { aS.volume *= volM; }
            aS.PlayOneShot(GetAudioClip(objectTag));
        }
        else if (sound.audioType == AudioType.sfx) 
        {
            poolable = ObjectPoolManager.Instance.SpawnObject(POOLNAME);
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
        
    }

    //used to play 2d menu audio
    public void Play2dAudioOnce(string objectTag)
    {
        sound = audioSoundsAudioClipDictionary[objectTag];
        buttonAudioSource.volume = (PlayerPrefManager.Instance.sfxVolume / 100) * sound.volMultiplier;
        buttonAudioSource.PlayOneShot(GetAudioClip(objectTag));
    }

    //returns the audio clip coresponding to the tag in the dictionary
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

    // method to play a 2d sound at a specific volume
    public void PlaySoundAtVolume(float vol, string objectTag)
    {
        buttonAudioSource.volume = vol/100;
        buttonAudioSource.PlayOneShot(GetAudioClip(objectTag));
    }

    // stops the music
    public void StopMusic()
    {
        musicAudioSource.Stop();
    }

    // pauses music
    public void PauseMusic()
    {
        musicAudioSource.Pause();
    }

    // resumes music
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
        sfxPlayer,
        other
    }
}