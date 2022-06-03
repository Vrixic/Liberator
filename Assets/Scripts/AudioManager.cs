using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public PoolableObject pAudioSource;
    /* list of sounds */
    public List<SoundEffect> sfxSounds = new List<SoundEffect>();

    public Dictionary<string, string> audioSoundDictionary = new Dictionary<string, string>();

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
            Debug.LogError("Multiple AudioManagers! Destroying the newest one: " + this.name);
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        foreach (SoundEffect sound in sfxSounds)
        {
            audioSoundDictionary.Add(sound.objectTag, ObjectPoolManager.Instance.CreateObjectPool(pAudioSource, 1));
            audioSoundsAudioClipDictionary.Add(sound.objectTag, sound.sound);
        }

        //foreach (Sound sound in uiSounds)
        //{
        //    audioSoundsAudioClipDictionary.Add(sound.objectTag, sound.audio);
        //}
    }

    public void PlayAudioAtLocation(Vector3 location, string objectTag)
    {
        PoolableObject obj;
        AudioSource ad;
        Sound sE;
        if (audioSoundsAudioClipDictionary.ContainsKey(objectTag))
        {
            obj = ObjectPoolManager.Instance.SpawnObject(audioSoundDictionary[objectTag]);
            sE = audioSoundsAudioClipDictionary[objectTag];
        }
        else
        {
            obj = ObjectPoolManager.Instance.SpawnObject(audioSoundDictionary[sfxSounds[0].objectTag]);
            sE = audioSoundsAudioClipDictionary[sfxSounds[0].objectTag];
        }

        obj.transform.position = location;
        ad = obj.GetComponent<AudioSource>();
        
        if (sE.audioType == AudioType.sfx) {
            ad.volume = (PlayerPrefManager.Instance.sfxVolume/100) * sE.volMultiplier; }
        else if (sE.audioType == AudioType.music) {
            ad.volume = (PlayerPrefManager.Instance.musicVolume/100) * sE.volMultiplier; }
        else if (sE.audioType == AudioType.other){
            ad.volume = (PlayerPrefManager.Instance.masterVolume/100) * sE.volMultiplier; }
        else { ad.volume = (PlayerPrefManager.Instance.masterVolume/100); }

        ad.PlayOneShot(GetAudioClip(objectTag));
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
        music, 
        other
    }
}