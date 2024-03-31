using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// HorseController handles horse audio and animation
/// Author: James Orson
/// </summary>

public class HorseController : MonoBehaviour
{
    #region Unity Components
    private AudioSource _gallopAudio;
    private Animator _animator;
    #endregion
    public float Velocity = 1f;

    private UnityAction<float> volumeChangeListener;

    #region Unity hooks
    private void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Goose"))
        {
            EventManager.TriggerEvent<AttackEvent, string, float, Collider>(AttackEvent.ATTACK_WITH_HORSE, 0f, c);
        }
    }

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _gallopAudio = GetComponent<AudioSource>();

        volumeChangeListener = new UnityAction<float>(VolumeChangeHandler);
        EventManager.StartListening<VolumeChangeEvent, float>(volumeChangeListener);
    }

    private void UpdateAnimation()
    {
        _animator.SetFloat("Speed", Velocity);
    }

    private void UpdateAudio()
    {
        if (Velocity > 0f && !_gallopAudio.isPlaying)
        {
            _gallopAudio.Play();
        }
        else if (Velocity <= 0f && _gallopAudio.isPlaying)
        {
            _gallopAudio.Stop();
        }
    }

    private void VolumeChangeHandler(float volume)
    {
        _gallopAudio.volume = volume;
    }

    private void FixedUpdate()
    {
        UpdateAnimation();
        UpdateAudio();
    }
    
    #endregion
}
