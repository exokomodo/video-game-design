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
    private float _preDeathVelocity;
    private bool _isDying = false;

    private UnityAction<float> volumeChangeListener;

    private void WhoaNelly()
    {
        _preDeathVelocity = Velocity;
        _isDying = true;
        EventManager.TriggerEvent<KillKittyEvent>();
    }

    #region Unity hooks
    private void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Goose"))
        {
            EventManager.TriggerEvent<AttackEvent, string, float, Collider>(AttackEvent.ATTACK_WITH_HORSE, 0f, c);
        }
        else if (c.CompareTag("Pond"))
        {
            WhoaNelly();
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
        if (_isDying)
        {
            Velocity = Mathf.Lerp(_preDeathVelocity, 0f, Time.deltaTime);
        }
        UpdateAnimation();
        UpdateAudio();
    }
    
    #endregion
}
