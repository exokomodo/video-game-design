// NOTE: Based on Assets/Scripts/AppEvents/AudioEventManager.cs in example project
using UnityEngine;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{

    private float volume = 1f;

    public AudioClip[] audioClips;

    public AudioClip tireStackBounceAudio;
    private UnityAction<Vector3> tireStackBounceListener;
    private UnityAction<Vector3, string> audioEventListener;

    #region Unity Hooks
    private void Awake()
    {
        tireStackBounceListener = new UnityAction<Vector3>(tireStackBounceEventHandler);
        audioEventListener = new UnityAction<Vector3, string>(audioEventHandler);
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

    void tireStackBounceEventHandler(Vector3 position)
    {
        Debug.Log(tireStackBounceAudio);
        AudioSource.PlayClipAtPoint(tireStackBounceAudio, position, 1f);
    }

    void audioEventHandler(Vector3 position, string clipName)
    {
        foreach (AudioClip audioClip in audioClips)
        {
            if (audioClip.name == clipName)
            {
                AudioSource.PlayClipAtPoint(audioClip, position, volume);
            }
        }
    }
}
