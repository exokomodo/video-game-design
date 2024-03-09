using KittyHawk.Extensions;
using UnityEngine;

/// <summary>
/// HorseController controls the horse on a waypoint-based path
/// Author: James Orson
/// </summary>

public class HorseController : MonoBehaviour
{
    #region Unity Components
    private AudioSource _gallopAudio;
    private GameObject _waypointRoot;
    private GameObject _carrot;
    private Animator _animator;
    private int _waypointIndex;
    #endregion
    public bool IsFollowingCarrot;
    public float Velocity = 1f;
    public float VelocityScale = 5f;
    public float RotationSpeed = 2f;

    #region Private methods
    private void FaceWaypoint()
    {
        var direction = Vector3.Normalize(
            GetWaypointTransform().position - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            Time.deltaTime * RotationSpeed);
    }
    private int GetNumberOfWaypoints() => _waypointRoot.transform.childCount;
    private void NextWaypoint() => _waypointIndex = (_waypointIndex + 1) % GetNumberOfWaypoints();
    private Transform GetWaypointTransform() => IsFollowingCarrot ? _carrot.transform : _waypointRoot.transform.GetChild(_waypointIndex);
    #endregion

    #region Unity hooks
    private void Start()
    {
        var waypoints = GameObject.FindGameObjectsWithTag("HorseWaypoints");
        if (waypoints.Length > 0)
        {
            _waypointRoot = waypoints[0];
        }
        _animator = GetComponentInChildren<Animator>();
        _gallopAudio = GetComponent<AudioSource>();
        _waypointIndex = 0;
        _carrot = gameObject.GetChildByTag("Carrot");
    }

    private void FixedUpdate()
    {
        transform.position += (
            Time.fixedDeltaTime *
            Velocity *
            VelocityScale *
            transform.forward);
    }

    private void OnTriggerEnter(Collider c)
    {
        if (IsFollowingCarrot)
        {
            // NOTE: Do not consider waypoints visited while following the carrot
            return;
        }
        if (c.CompareTag("HorseWaypoint"))
        {
            NextWaypoint();
        }
    }

    private void UpdateAnimation()
    {
        _animator.SetFloat("Speed", Velocity);
        FaceWaypoint();
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
