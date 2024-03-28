using System.Linq;
using UnityEngine;

/// <summary>
/// Saddle A component that can be added to a GameObject to make it saddled, making the object into a hat
/// Author: James Orson
/// </summary>
public class Saddle : MonoBehaviour
{
    #region Public fields
    public Vector3 SaddleHeight = Vector3.zero;
    [KittyHawk.Attributes.TagSelector]
    public string RiderTag = "Player";
    public GameObject CarrotForward;
    public GameObject CarrotLeft;
    public GameObject CarrotRight;
    #endregion

    #region Protected properties
    protected InputReader _input;
    protected GameObject _rider = null;
    protected PlayerController _playerController = null;
    protected WaypointAI _waypointAI;
    #endregion

    #region Unity hooks
    private void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag(RiderTag))
        {
            Debug.Log("Saddle enter");
            Debug.Log("Rider entered");
            _rider = GameObject.FindGameObjectsWithTag("Player")
                .FirstOrDefault(x => x.GetComponent<PlayerController>());
            EventManager.TriggerEvent<RiderEnterEvent, Saddle, GameObject>(
                this,
                _rider);
            _playerController = _rider.GetComponent<PlayerController>();
            _waypointAI.SetCarrot(CarrotForward);
            _playerController.ToggleActive(false);
            _input.JumpEvent += Dismount;
        }
    }

    private void TurnLeft()
    {
        _waypointAI.SetCarrot(CarrotLeft);
    }

    private void TurnRight()
    {
        _waypointAI.SetCarrot(CarrotRight);
    }

    private void TurnStraight()
    {
        _waypointAI.SetCarrot(CarrotForward);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(SaddleHeight, 0.1f);
    }

    private void Update()
    {
        if (_rider != null)
        {
            _rider.transform.position = transform.position + SaddleHeight;
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

    private void Dismount()
    {
        _input.JumpEvent -= Dismount;
        EventManager.TriggerEvent<RiderExitEvent, Saddle, GameObject>(
            this,
            _rider);
        _playerController.ToggleActive(true);
        _waypointAI.SetCarrot(null);
        _rider = null;
    }

    private void Start()
    {
        _input = GetComponent<InputReader>();
        _waypointAI = GetComponent<WaypointAI>();
        Debug.Assert(GetComponent<Rigidbody>() != null, "Saddle must have a rigidbody");
        Debug.Assert(GetComponents<Collider>().Length > 0, "Saddle must at least one collider");
        Debug.Assert(_waypointAI != null, "Saddle must have a WaypointAI component");
    }
    #endregion
}
