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
    public float Speed = 1f;
    private bool _isSlowing = false;

    private UnityAction<float> volumeChangeListener;

    private void Trample(GameObject goose)
    {
        var gooseAi = goose.GetComponent<GooseAI>();
        if (gooseAi.IsAlive && saddle.HasRider)
        {
            gooseAi.Die();
            EventManager.TriggerEvent<HorseTrampleGooseEvent>();
        }
    }

    private void WhoaNelly()
    {
        _isSlowing = true;
    }

    #region Unity hooks
    private void OnTriggerEnter(Collider c)
    {
        Debug.Log($"Horse hit a collider of type({c.GetType()}) on game object with name({c.gameObject.name}) and tag({c.gameObject.tag})");
        if (c.CompareTag("Pond"))
        {
            EventManager.TriggerEvent<HorseEnterPondEvent>();
        }
        else if (c.CompareTag("Goose"))
        {
            Debug.Log($"Hit goose with a collider of type: {c.GetType()}");
            Trample(c.gameObject);
        }
    }

    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("Goose"))
        {
            Trample(c.gameObject);
        }
    }

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _gallopAudio = GetComponent<AudioSource>();
        saddle = GetComponent<Saddle>();

        volumeChangeListener = new UnityAction<float>(VolumeChangeHandler);
        EventManager.StartListening<VolumeChangeEvent, float>(volumeChangeListener);
        EventManager.StartListening<HorseEnterPondEvent>(WhoaNelly);
    }

    private void UpdateAnimation()
    {
        _animator.SetFloat("Speed", 1);
    }

    private void UpdateAudio()
    {
        if (Speed > 0f && !_gallopAudio.isPlaying)
        {
            _gallopAudio.Play();
        }
        else if (Speed <= 0f && _gallopAudio.isPlaying)
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
            Speed = Mathf.Clamp(Speed - Time.fixedDeltaTime, 0f, 1f);
            if (Speed <= 0.1f && saddle.IsMounted)
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
