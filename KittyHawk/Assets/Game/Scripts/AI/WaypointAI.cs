using UnityEngine;

/// <summary>
/// WaypointAI controls a game object on a waypoint-based path
/// Author: James Orson
/// </summary>

[RequireComponent(typeof(GameObject), typeof(GameObject))]
public class WaypointAI : MonoBehaviour
{
    #region Unity Components
    public GameObject WaypointRoot;
    public GameObject Carrot;
    #endregion
    public float Velocity = 1f;
    public float RotationSpeed = 2f;
    [KittyHawk.Attributes.TagSelector]
    public string WaypointTag = "";

    private int _waypointIndex = 0;

    #region Public methods
    public void SetCarrot(GameObject carrot) => Carrot = carrot;
    #endregion

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
    private Transform GetWaypointTransform() => Carrot != null ? Carrot.transform : WaypointRoot.transform.GetChild(_waypointIndex);
    #endregion

    #region Unity hooks
    private void Start()
    {
        _waypointIndex = 0;

        Debug.Assert(WaypointRoot != null, "WaypointRoot is null");
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
        if (Carrot != null)
        {
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
