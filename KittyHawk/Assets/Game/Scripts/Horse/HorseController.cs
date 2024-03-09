using UnityEngine;

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

    #region Unity hooks
    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _gallopAudio = GetComponent<AudioSource>();
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

    private void Update()
    {
        UpdateAnimation();
        UpdateAudio();

    }
    #endregion
}
