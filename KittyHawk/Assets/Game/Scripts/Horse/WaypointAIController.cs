using KittyHawk.Extensions;
using UnityEngine;

/// <summary>
/// WaypointAIController controls a game object on a waypoint-based path
/// Author: James Orson
/// </summary>

[RequireComponent(typeof(GameObject))]
public class WaypointAIController : MonoBehaviour
{
    #region Unity Components
    private int _waypointIndex = 0;
    #endregion
    public GameObject WaypointRoot;
    public GameObject Carrot;
    public bool IsFollowingCarrot = false;
    public float Velocity = 1f;
    public float RotationSpeed = 2f;
    // Restrict the field to available tag values
    [KittyHawk.Attributes.TagSelector]
    public string WaypointTag = "";

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
    private int GetNumberOfWaypoints() => WaypointRoot.transform.childCount;
    private void NextWaypoint() => _waypointIndex = (_waypointIndex + 1) % GetNumberOfWaypoints();
    private Transform GetWaypointTransform() => IsFollowingCarrot ? Carrot.transform : WaypointRoot.transform.GetChild(_waypointIndex);
    #endregion

    #region Unity hooks
    private void Start()
    {
        _waypointIndex = 0;

        Debug.Assert(WaypointRoot != null, "WaypointRoot is null");
        Debug.Assert(Carrot != null, "Carrot is null");
    }

    private void FixedUpdate()
    {
        transform.position += (
            Time.fixedDeltaTime *
            5f * Velocity *
            transform.forward);
        transform.position = new Vector3(
            transform.position.x,
            0,
            transform.position.z);
    }

    private void OnTriggerEnter(Collider c)
    {
        if (IsFollowingCarrot)
        {
            // NOTE: Do not consider waypoints visited while following the carrot
            return;
        }
        if (c.CompareTag(WaypointTag))
        {
            NextWaypoint();
        }
    }

    private void Update() => FaceWaypoint();
    #endregion
}
