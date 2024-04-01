using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// HorseController handles horse audio and animation
/// Author: James Orson
/// </summary>

public class HorseController : MonoBehaviour
{
    #region Unity Components
    public Saddle saddle;
    private AudioSource _gallopAudio;
    private Animator _animator;
    #endregion
    public float Velocity = 1f;
    private bool _isSlowing = false;

    private UnityAction<float> volumeChangeListener;

    private void WhoaNelly()
    {
        _isSlowing = true;
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
        saddle = GetComponent<Saddle>();

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
        if (_isSlowing)
        {
            Velocity = Mathf.Clamp(Velocity - Time.fixedDeltaTime, 0f, 1f);
            if (Velocity <= 0.1f && saddle.IsMounted)
            {
                EventManager.TriggerEvent<KillKittyEvent>();
                _isSlowing = false;
            }
        }
        UpdateAnimation();
        UpdateAudio();
    }
    
    #endregion
}
