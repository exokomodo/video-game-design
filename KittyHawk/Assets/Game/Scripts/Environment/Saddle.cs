using System.Linq;
using Cinemachine;
using UnityEngine;

/// <summary>
/// Saddle A component that can be added to a GameObject to make it saddled, making the object into a hat
/// Author: James Orson
/// </summary>
public class Saddle : MonoBehaviour
{
    #region Public fields
    public Vector3 SaddleOffset = Vector3.zero;
    public GameObject CinemachineVirtualCamera;
    [KittyHawk.Attributes.TagSelector]
    public string RiderTag = "Player";
    public GameObject CarrotForward;
    public GameObject CarrotLeft;
    public GameObject CarrotRight;
    public float RidingFov = 70f;
    public float RidingRigOrbitOffset = 1f;
    #endregion

    #region Protected properties
    protected CinemachineFreeLook _cinemachineFreeLook = null;
    protected InputReader _input;
    protected GameObject _rider = null;
    protected PlayerController _playerController = null;
    protected WaypointAI _waypointAI = null;
    protected Transform _oldFollow = null;
    protected Transform _oldLookAt = null;
    protected float _oldFov = 0f;
    #endregion

    #region Protected methods
    protected void TurnLeft()
    {
        _waypointAI.SetCarrot(CarrotLeft);
    }

    protected void TurnRight()
    {
        _waypointAI.SetCarrot(CarrotRight);
    }

    protected void TurnStraight()
    {
        _waypointAI.SetCarrot(CarrotForward);
    }

    protected void Mount()
    {
        if (_rider != null)
        {
            return;
        }
        _rider = GameObject.FindGameObjectsWithTag("Player")
            .FirstOrDefault(x => x.GetComponent<PlayerController>());
        EventManager.TriggerEvent<RiderEnterEvent, Saddle, GameObject>(
            this,
            _rider);
        _playerController = _rider.GetComponent<PlayerController>();
        _waypointAI.SetCarrot(CarrotForward);
        _playerController.ToggleActive(false);
        if (_cinemachineFreeLook != null)
        {
            _oldFollow = _cinemachineFreeLook.Follow;
            _oldLookAt = _cinemachineFreeLook.LookAt;
            _cinemachineFreeLook.Follow = CarrotForward.transform;
            _cinemachineFreeLook.LookAt = CarrotForward.transform;
            AdjustRigHeights(SaddleOffset.y);
            AdjustRigOrbits(RidingRigOrbitOffset);
            AdjustFov(RidingFov);
        }
    }

    protected void Dismount()
    {
        _input.JumpEvent -= Dismount;
        EventManager.TriggerEvent<RiderExitEvent, Saddle, GameObject>(
            this,
            _rider);
        _playerController.ToggleActive(true);
        _waypointAI.SetCarrot(null);
        if (_cinemachineFreeLook != null)
        {
            AdjustFov(_oldFov);
            AdjustRigOrbits(-RidingRigOrbitOffset);
            AdjustRigHeights(-SaddleOffset.y);
            _cinemachineFreeLook.Follow = _oldFollow;
            _cinemachineFreeLook.LookAt = _oldLookAt;
        }
        _rider = null;
    }

    protected void AdjustFov(float fov)
    {
        _cinemachineFreeLook.m_Lens.FieldOfView = fov;
    }

    protected void AdjustRigHeights(float height)
    {
        for (int i = 0; i < 3; i++)
        {
            _cinemachineFreeLook.m_Orbits[i].m_Height += height;
        }
    }

    protected void AdjustRigOrbits(float radius)
    {
        for (int i = 0; i < 3; i++)
        {
            _cinemachineFreeLook.m_Orbits[i].m_Radius += radius;
        }
    }
    #endregion

    #region Unity hooks
    private void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag(RiderTag) && _rider == null)
        {
            Mount();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(SaddleOffset, 0.1f);
    }

    private void Update()
    {
        if (_rider != null)
        {
            _rider.transform.position = transform.position + SaddleOffset;
            _rider.transform.rotation = transform.rotation;
            Debug.Log("Movement value:" + _input.MovementValue);
            if (_input.MovementValue.x < -0.2f)
            {
                TurnLeft();
            }
            else if (_input.MovementValue.x > 0.2f)
            {
                TurnRight();
            }
            else
            {
                TurnStraight();
            }
        }
    }

    private void Start()
    {
        _input = GetComponent<InputReader>();
        _waypointAI = GetComponent<WaypointAI>();
        Debug.Assert(GetComponent<Rigidbody>() != null, "Saddle must have a rigidbody");
        Debug.Assert(GetComponents<Collider>().Length > 0, "Saddle must at least one collider");
        Debug.Assert(_waypointAI != null, "Saddle must have a WaypointAI component");
        if (CinemachineVirtualCamera != null)
        {
            _cinemachineFreeLook = CinemachineVirtualCamera.GetComponent<Cinemachine.CinemachineFreeLook>();
        }
    }
    #endregion
}
