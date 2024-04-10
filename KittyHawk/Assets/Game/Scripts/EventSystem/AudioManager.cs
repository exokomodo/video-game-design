// NOTE: Based on Assets/Scripts/AppEvents/AudioEventManager.cs in example project
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// AudioManager allows for triggering of audio events and audio on general events
/// Author: James Orson
/// </summary>

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance
    {
        get
        {
            if (!audioManager)
            {
                audioManager = FindObjectOfType(typeof(AudioManager)) as AudioManager;

                if (!audioManager)
                {
                    Debug.LogError("There needs to be one active AudioManager script on a GameObject in your scene.");
                }
            }

            return audioManager;
        }
    }
    [SerializeField]
    string pathPrefix = "Sound";
    private float soundVolume = 1f;
    private float musicVolume = 1f;
    private Dictionary<string, AudioClip> soundEffects;
    private UnityAction<Vector3, string> audioEventListener;
    private AudioClip tireStackBounceAudio;
    private UnityAction<Vector3> tireStackBounceListener;
    private UnityAction<string> musicEventListener;
    private AudioSource musicSource;
    private static AudioManager audioManager;

    #region Public Setters

    public float SoundVolume
    {
        get { return soundVolume; }
        set
        {
            if (value < 0)
            {
                soundVolume = 0f;
            }
            else if (value > 1)
            {
                soundVolume = 1f;
            }
            else
            {
                soundVolume = value;
                EventManager.TriggerEvent<VolumeChangeEvent, float>(soundVolume);
                DataManager.Instance.SoundVolume = soundVolume;
            }
        }
    }

    public float MusicVolume
    {
        get { return musicVolume;  }
        set
        {
            if (value < 0)
            {
                musicVolume = 0f;
            }
            else if (value > 1)
            {
                musicVolume = 1f;
            }
            else
            {
                musicVolume = value;
                musicSource.volume = musicVolume;
                EventManager.TriggerEvent<MusicVolumeChangeEvent, float>(musicVolume);
                DataManager.Instance.MusicVolume = musicVolume;
            }
        }
    }

    #endregion

    #region Unity Hooks
    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {

        EventManager.StartListening<TireStackBounceEvent, Vector3>(tireStackBounceListener);
        EventManager.StartListening<AudioEvent, Vector3, string>(audioEventListener);
        EventManager.StartListening<MusicEvent, string>(musicEventListener);

    }

    private void OnDisable()
    {
        EventManager.StopListening<TireStackBounceEvent, Vector3>(tireStackBounceListener);
        EventManager.StopListening<AudioEvent, Vector3, string>(audioEventListener);
        EventManager.StopListening<MusicEvent, string>(musicEventListener);
    }
    #endregion

    #region Event Handlers

    void tireStackBounceEventHandler(Vector3 position)
    {
        Debug.Log(tireStackBounceAudio);
        AudioSource.PlayClipAtPoint(tireStackBounceAudio, position, soundVolume);
    }

    void audioEventHandler(Vector3 position, string clipName)
    {
        if (!soundEffects.TryGetValue(clipName, out AudioClip clip))
        {
            clip = LoadAudioClip(clipName);
        }
        AudioSource.PlayClipAtPoint(clip, position, soundVolume);
    }

    // TODO: Actually implement music event handler
    // Currently only playing music on awake in level

    void musicEventHandler(string clipName)
    {
        if (!soundEffects.TryGetValue(clipName, out AudioClip clip))
        {
            clip = LoadAudioClip(clipName);
        }
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }

    #endregion

    #region Private Methods
    private void Init()
    {
        tireStackBounceListener = new UnityAction<Vector3>(tireStackBounceEventHandler);
        audioEventListener = new UnityAction<Vector3, string>(audioEventHandler);
        musicEventListener = new UnityAction<string>(musicEventHandler);
        soundEffects = new Dictionary<string, AudioClip>();
        tireStackBounceAudio = LoadAudioClip("tire-stack-bounce");
        musicSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // This can't go in Init since it's called by Awake
        SoundVolume = DataManager.Instance.SoundVolume;
        MusicVolume = DataManager.Instance.MusicVolume;
    }

    private AudioClip LoadAudioClip(string name)
    {
        var clip = Resources.Load<AudioClip>(Path.Join(pathPrefix, name));
        soundEffects.Add(name, clip);
        return clip;
    }
    #endregion
}
