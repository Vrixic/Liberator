using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    /* list of sounds */
    public List<Sound> gameSounds = new List<Sound>();

    public List<Sound> uiSounds = new List<Sound>();

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
            audioSoundsAudioClipDictionary.Add(sound.objectTag, sound.audio);
        }

        foreach (Sound sound in uiSounds)
        {
            audioSoundsAudioClipDictionary.Add(sound.objectTag, sound.audio);
        }
    }

    public void SetAudioSource(AudioSource audioSource)
    {
        m_AudioSource = audioSource;
    }

    public void PlayAudioAtLocation(AudioSource audioSource, string objectTag, float volume = 1f)
    {
        audioSource.volume = PlayerPrefManager.Instance.masterVolume;

        //SetAudioSource(audioSource);
        audioSource.PlayOneShot(GetAudioClip(objectTag));
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
            return audioSoundsAudioClipDictionary["Untagged"];
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