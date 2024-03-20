using System;
using UnityEngine;
using UnityEngine.AI;

// FSM adapted from https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/
public class Bunny : MonoBehaviour
{
  private Rigidbody rb;
  private CapsuleCollider col;
  private int currWaypoint = 0;
  private FiniteStateMachine<Bunny> FSM;
  private float verticalVelocity = 0f;
  private float groundCheckTolerance = 0.1f;
  private Vector3 pendingMotion;

  public NavMeshAgent agent;
  public Animator anim;
  public GameObject[] waypoints;
  public bool followMode = false;
  public GameObject followTarget;
  public enum BunnyAnimState
  {
    IDLE,
    MOVE,
    JUMP
  }

  public enum BunnyState
  {
    PATROL,
    PURSUE,
    FOLLOW
  }

  public bool UpdateAgent {
    get {
      return agent.updatePosition;
    }
    set {
      agent.updatePosition = value;
    }
  }

  public Vector3 velocity => rb.velocity;
  public bool isGrounded => CheckGrounded();

  public void Awake()
  {
    rb = GetComponent<Rigidbody>();
    if (rb == null) throw new Exception("Rigid body could not be found");

    col = GetComponent<CapsuleCollider>();
    if (col == null) throw new Exception("Collider could not be found");

    FSM = new FiniteStateMachine<Bunny>();
    FSM.Configure(this, GetState());
  }

  public void ChangeState(FSMState<Bunny> e)
  {
    FSM.ChangeState(e);
  }

  public void SetNextWaypoint()
  {
      ++currWaypoint;
      if (currWaypoint >= waypoints.Length) currWaypoint = 0;
      ChangeState(GetState());
  }

  public void SetAgentDestination(Vector3? targetPos = null)
  {
    Vector3 pos = targetPos == null? CurrentWaypoint.transform.position : (Vector3)targetPos;
    if (pos != null && agent.enabled) agent.SetDestination(pos);
  }

  public GameObject CurrentWaypoint
  {
    get {
      try
      {
        return waypoints[currWaypoint];
      }
      catch (Exception e)
      {
          Debug.LogError("Error: Could not set NavMeshAgent Destination: " + e.Message);
      }
      return null;
    }
  }

  private FSMState<Bunny> GetState()
  {
    return followMode? BunnyFollowState.Instance : PatrolState.Instance;
  }

  private void FixedUpdate()
    {
        // Debug.Log("ForceReceiver FixedUpdate verticalVelocity: " + verticalVelocity);
      if (verticalVelocity < 0f && CheckGrounded())
      {
          verticalVelocity = Physics.gravity.y * Time.fixedDeltaTime;
      }
      else
      {
          verticalVelocity += Physics.gravity.y * Time.fixedDeltaTime;
      }
      // rb.AddForce(jumpVector, ForceMode.Acceleration);
      FSM.Update();
    }

  void OnAnimatorMove()
  {
    // Vector3 newRootPosition = anim.rootPosition + new Vector3(0, anim.rootPosition.y + verticalVelocity, 0);
    // Vector3 jumpVector = new Vector3(0, verticalVelocity, 0);
    Vector3 newRootPosition = (rb.velocity + pendingMotion) * Time.fixedDeltaTime;
    newRootPosition.y = verticalVelocity;
    pendingMotion = Vector3.zero;
    rb.velocity = newRootPosition;

  }

  public bool CheckGrounded()
  {
    Vector3 pos = rb.transform.position;
    Vector3 origin = new Vector3(pos.x, pos.y + 0.01f, pos.z);
    return RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(origin, Vector3.down, groundCheckTolerance, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Both);
  }


  public void Move(Vector3 motion)
  {
    pendingMotion = motion;
    verticalVelocity = motion.y;
  }

  public void Jump(float jumpForce)
  {
    pendingMotion = new Vector3(0, jumpForce, 0);
    verticalVelocity = jumpForce;
  }
}
