using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public PoolableObject pAudioSource;
    /* list of sounds */
    public List<Sound> gameSounds = new List<Sound>();

    public List<Sound> uiSounds = new List<Sound>();

    public Dictionary<string, string> audioSoundDictionary = new Dictionary<string, string>();

    public Dictionary<string, AudioClip> audioSoundsAudioClipDictionary = new Dictionary<string, AudioClip>();

    AudioSource m_AudioSource;

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
            Debug.LogError("Multiple AudioManagers! Destroying the newest one: " + this.name);
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        foreach (Sound sound in gameSounds)
        {
            audioSoundDictionary.Add(sound.objectTag, ObjectPoolManager.Instance.CreateObjectPool(pAudioSource, 1));
            audioSoundsAudioClipDictionary.Add(sound.objectTag, sound.audio);
        }

        //foreach (Sound sound in uiSounds)
        //{
        //    audioSoundsAudioClipDictionary.Add(sound.objectTag, sound.audio);
        //}
    }

    public void SetAudioSource(AudioSource audioSource)
    {
        m_AudioSource = audioSource;
    }

    public void PlayAudioAtLocation(Vector3 location, string objectTag, float volume = 1f)
    {
        PoolableObject obj;
        if (audioSoundsAudioClipDictionary.ContainsKey(objectTag))
        {
            obj = ObjectPoolManager.Instance.SpawnObject(audioSoundDictionary[objectTag]);
        }
        else
        {
            obj = ObjectPoolManager.Instance.SpawnObject(audioSoundDictionary["Untagged"]);
        }

        //SetAudioSource(audioSource);
        obj.transform.position = location;
        AudioSource ad = obj.GetComponent<AudioSource>();
        ad.volume = PlayerPrefManager.Instance.masterVolume;
        ad.PlayOneShot(GetAudioClip(objectTag));
    }

    public void SetAudioVolume(float val)
    {
        m_AudioSource.volume = val;
    }

    public AudioClip GetAudioClip(string objectTag)
    {
        if (audioSoundsAudioClipDictionary.ContainsKey(objectTag))
        {
            return audioSoundsAudioClipDictionary[objectTag];
        }
        else
        {
            return audioSoundsAudioClipDictionary[gameSounds[0].objectTag];
        }
    }

    /*
     * Stores the tag and the particle system associated with the tag
     */
    [System.Serializable]
    public class Sound
    {
        public string objectTag;
        public AudioClip audio;
    }

    public enum AudioPurpose
    {
        Untagged,
        FootStep,
        LandingFromJump,
        Pickup,
        Hostage,
        XpGain,
        ButtonHover,
        ButtonClick,
        MenuMusic,
    }
}