using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusicScript : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioClip menuMusic;
    [SerializeField] AudioClip gameMusic;

    float volMultiplyer = 0.1f;
    // Start is called before the first frame update
    private static MenuMusicScript m_Instance;
    public static MenuMusicScript Instance
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
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        //PlayMenuMusic();
        DontDestroyOnLoad(this.gameObject);
    }
    private void Update()
    {
        musicSource.volume = (PlayerPrefManager.Instance.musicVolume / 100) * volMultiplyer;
    }

    public void PlayMenuMusic()
    {
        musicSource.clip = menuMusic;
        volMultiplyer = 0.1f;
        musicSource.volume = (PlayerPrefManager.Instance.musicVolume / 100) * volMultiplyer;
        musicSource.Play();
    }
    public void StopMenuMusic()
    {
        musicSource.Stop();
    }

    public void PlayGameMusicDelayed(float time)
    {
        Invoke("PlayGameMusic", time);
    }
    public void PlayGameMusic()
    {
        musicSource.clip = gameMusic;
        volMultiplyer = 0.05f;
        musicSource.volume = (PlayerPrefManager.Instance.musicVolume / 100) * volMultiplyer;
        musicSource.Play();
    }
    
    public void PauseGameMusic()
    {
        musicSource.Pause();
    }

}
