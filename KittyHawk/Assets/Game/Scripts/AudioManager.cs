// NOTE: Based on Assets/Scripts/AppEvents/AudioEventManager.cs in example project
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{

    private float volume = 1f;

    public AudioClip[] audioClips;

    public AudioClip tireStackBounceAudio;
    private UnityAction<Vector3> tireStackBounceListener;
    private UnityAction<Vector3, string> audioEventListener;

    private Dictionary<string, AudioClip> soundEffects;

    #region Public Setters

    public float Volume
    {
        get { return volume; }
        set
        {
            if (value < 0)
            {
                volume = 0f;
            }
            else if (value > 1)
            {
                volume = 1f;
            }
            else
            {
                volume = value;
            }
        }
    }

    #endregion

    #region Unity Hooks
    private void Awake()
    {
        tireStackBounceListener = new UnityAction<Vector3>(tireStackBounceEventHandler);
        audioEventListener = new UnityAction<Vector3, string>(audioEventHandler);

        soundEffects = new Dictionary<string, AudioClip>();

        foreach (AudioClip audioClip in audioClips)
        {
            soundEffects.Add(audioClip.name, audioClip);
        }
    }

    private void OnEnable()
    {

        EventManager.StartListening<TireStackBounceEvent, Vector3>(tireStackBounceListener);
        EventManager.StartListening<AudioEvent, Vector3, string>(audioEventListener);

    }

    private void OnDisable()
    {
        EventManager.StopListening<TireStackBounceEvent, Vector3>(tireStackBounceListener);
        EventManager.StopListening<AudioEvent, Vector3, string>(audioEventListener);
    }
    #endregion

    #region Event Handlers

    void tireStackBounceEventHandler(Vector3 position)
    {
        Debug.Log(tireStackBounceAudio);
        AudioSource.PlayClipAtPoint(tireStackBounceAudio, position, 1f);
    }

    void audioEventHandler(Vector3 position, string clipName)
    {

        if (soundEffects[clipName] != null)
        {
            AudioSource.PlayClipAtPoint(soundEffects[clipName], position, volume);
        }

    }

    #endregion
}
