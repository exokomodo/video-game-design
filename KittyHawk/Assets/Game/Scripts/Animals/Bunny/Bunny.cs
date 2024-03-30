using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// A Bunny Controller that manages the Bunny model, finite state machine,
/// animation controller, events. Offers follow mode, waypoint patrolling.
/// Author: Geoffrey Roth
/// </summary>
public class Bunny : MonoBehaviour
{
  protected Rigidbody rb;
  protected CapsuleCollider col;
  protected int currWaypoint = 0;
  protected FiniteStateMachine<Bunny> FSM;
  protected float verticalVelocity = 0f;
  protected float groundCheckTolerance = 0.1f;
  protected Vector3 pendingMotion;
  protected int MagnitudeHash = Animator.StringToHash("Magnitude");
  protected int VelocityXHash = Animator.StringToHash("VelocityX");
  protected int VelocityZHash = Animator.StringToHash("VelocityZ");
  protected AgentLinkMover mover;

  public NavMeshAgent agent;
  public Animator anim;
  // public GameObject[] waypoints;
  public List<GameObject> Waypoints;
  public bool followMode = false;
  public GameObject followTarget;
  // [SerializeField]
  // public GameObject followTarget;
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

  public Vector3 position {
    get {
      return agent.transform.position;
    }
    set {
      agent.Warp(value);
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

    // followTarget = null;
    // try {
    //   followTarget = FindObjectsByType<FollowTarget>(FindObjectsSortMode.None)[0].gameObject;
    // } catch (Exception e) {
    //   Debug.LogWarning($"No followTarget found for Bunny. followMode is set to false. \n{e.Message}");
    //   followMode = false;
    // }
    if (followTarget == null) followMode = false;

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
      if (Waypoints.Count < 2) ChangeState(BunnyIdleState.Instance);
      if (++currWaypoint >= Waypoints.Count) currWaypoint = 0;
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
        return Waypoints[currWaypoint];
      }
      catch (Exception e)
      {
          Debug.LogError("Error: Could not set NavMeshAgent Destination: " + e.Message);
      }
      return null;
    }
  }

  protected BunnyBaseState GetState()
  {
    if (followMode && followTarget != null)
    {
      return BunnyFollowState.Instance;
    }
    if (Waypoints.Count > 0)
    {
      return BunnyPatrolState.Instance;
    }
    return BunnyIdleState.Instance;
  }

  protected void FixedUpdate()
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

  // protected void OnAnimatorMove()
  // {
  //   float velx = anim.GetFloat(VelocityXHash);
  //   float velz = anim.GetFloat(VelocityZHash);

  //   Vector3 newRootVelocity = new Vector3(velx * agent.speed, verticalVelocity, velz * agent.speed) + pendingMotion;
  //   pendingMotion = Vector3.zero;
  //   rb.velocity = newRootVelocity;
  // }

  public bool CheckGrounded()
  {
    Vector3 pos = rb.transform.position;
    Vector3 origin = new Vector3(pos.x, pos.y + 0.01f, pos.z);
    return RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(origin, Vector3.down, groundCheckTolerance, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor);
  }

  public void Move(Vector3 motion)
  {
    verticalVelocity = motion.y;
    pendingMotion = motion;
    pendingMotion.y = 0;
  }

  public void Patrol() {
    followMode = false;
    ChangeState(GetState());
  }

  public void Follow(GameObject target) {
    followMode = true;
    followTarget = target;
    ChangeState(GetState());
  }
}
