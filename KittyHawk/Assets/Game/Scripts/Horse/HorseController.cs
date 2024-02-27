using UnityEngine;

public class HorseController : MonoBehaviour
{
    #region Unity Components
    private GameObject _waypointRoot;
    private Animator _animator;
    private int _waypointIndex;
    #endregion
    public float Velocity = 0f;
    public const float VelocityScale = 5f;

    #region Private methods
    private void FaceWaypoint()
    {
        var direction = Vector3.Normalize(
            GetWaypointTransform().position - transform.position);
        transform.forward = new Vector3(
            direction.x,
            0f,
            direction.z);
    }
    private int GetNumberOfWaypoints() => _waypointRoot.transform.childCount;
    private void NextWaypoint() => _waypointIndex = (_waypointIndex + 1) % GetNumberOfWaypoints();
    private Transform GetWaypointTransform() => _waypointRoot.transform.GetChild(_waypointIndex);
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
        _waypointIndex = 0;
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
        Debug.Log("Touched waypoint");
        if (c.CompareTag("HorseWaypoint"))
        {
            NextWaypoint();
        }
    }

    private void Update()
    {
        _animator.SetFloat("Speed", Velocity);
        FaceWaypoint();
    }
    #endregion
}
