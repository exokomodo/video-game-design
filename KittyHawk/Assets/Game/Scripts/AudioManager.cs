// NOTE: Based on Assets/Scripts/AppEvents/AudioEventManager.cs in example project
using UnityEngine;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{
    public AudioClip tireStackBounceAudio;
    private UnityAction<Vector3> tireStackBounceListener;

    #region Unity Hooks
    private void Awake()
    {
        tireStackBounceListener = new UnityAction<Vector3>(tireStackBounceEventHandler);
    }

    private void OnEnable()
    {

        EventManager.StartListening<TireStackBounceEvent, Vector3>(tireStackBounceListener);

    }

    private void OnDisable()
    {
        EventManager.StopListening<TireStackBounceEvent, Vector3>(tireStackBounceListener);
    }
    #endregion 

    void tireStackBounceEventHandler(Vector3 position)
    {
        Debug.Log(tireStackBounceAudio);
        AudioSource.PlayClipAtPoint(tireStackBounceAudio, position, 1f);
    }
}
