using System;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// A Bunny Controller that manages the Bunny model, finite state machine,
/// animation controller, events. Offers follow mode, waypoint patrolling.
/// Author: Geoffrey Roth
/// </summary>
public class Bunny : MonoBehaviour
{
  private Rigidbody rb;
  private CapsuleCollider col;
  private int currWaypoint = 0;
  private FiniteStateMachine<Bunny> FSM;
  private float verticalVelocity = 0f;
  private float groundCheckTolerance = 0.1f;
  private Vector3 pendingMotion;
  protected int MagnitudeHash = Animator.StringToHash("Magnitude");
  protected int VelocityXHash = Animator.StringToHash("VelocityX");
  protected int VelocityZHash = Animator.StringToHash("VelocityZ");
  private AgentLinkMover mover;

  public NavMeshAgent agent;
  public Animator anim;
  public GameObject[] waypoints;
  public bool followMode = false;
  public GameObject followTarget { get; private set; }
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

  public float LinkMoveDuration
  {
    get {
      return mover.m_LinkMoveDuration;
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

    followTarget = null;
    try {
      followTarget = FindObjectsByType<FollowTarget>(FindObjectsSortMode.None)[0].gameObject;
    } catch (Exception e) {
      Debug.LogWarning($"No followTarget found for Bunny. followMode is set to false. \n{e.Message}");
      followMode = false;
    }
    mover = GetComponent<AgentLinkMover>();
    if (mover == null) throw new Exception("AgentLinkMover not found");

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

  private BunnyBaseState GetState()
  {
    if (followMode && followTarget != null)
    {
      return BunnyFollowState.Instance;
    }
    if (waypoints.Length > 0)
    {
      return BunnyPatrolState.Instance;
    }
    return BunnyIdleState.Instance;
  }

  private void FixedUpdate()
    {
      if (verticalVelocity < 0f && CheckGrounded())
      {
          verticalVelocity = Physics.gravity.y * Time.fixedDeltaTime;
      }
      else
      {
          verticalVelocity += Physics.gravity.y * Time.fixedDeltaTime;
      }
      FSM.Update();
    }

  void OnAnimatorMove()
  {
    float velx = anim.GetFloat(VelocityXHash);
    float velz = anim.GetFloat(VelocityZHash);

    Vector3 newRootVelocity = new Vector3(velx * agent.speed, verticalVelocity, velz * agent.speed) + pendingMotion;
    pendingMotion = Vector3.zero;
    rb.velocity = newRootVelocity;
    // Quaternion newRootRotation = Quaternion.LookRotation(new Vector3(rb.rotation.x, 0, rb.rotation.z), new Vector3(rb.rotation.x, 1, rb.rotation.z));
    // rb.MoveRotation(newRootRotation);
  }

  public bool CheckGrounded()
  {
    Vector3 pos = rb.transform.position;
    Vector3 origin = new Vector3(pos.x, pos.y + 0.01f, pos.z);
    return RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(origin, Vector3.down, groundCheckTolerance, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Both);
  }

  public void Move(Vector3 motion)
  {
    verticalVelocity = motion.y;
    pendingMotion = motion;
    pendingMotion.y = 0;
  }
}
