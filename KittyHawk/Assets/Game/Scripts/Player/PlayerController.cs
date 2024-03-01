using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {
  private Animator anim;
  private Rigidbody rb;
  private InputReader input;
  private PlayerStateMachine stateMachine;
  private CapsuleCollider col;
  public Vector3 velocity => stateMachine.Animator.velocity;

  private GameObject frontPivot;
  private GameObject backPivot;
  private bool _isGrounded = true;
  private bool _prevGrounded = true;
  private bool _isJumping = false;
  private bool _jump = false;
  private bool _isFalling = false;
  private bool _isRunning = false;
  private float groundCheckTolerance = 0.01f;
  private readonly int isRunningHash = Animator.StringToHash("isRunning");
  private readonly int isGroundedHash = Animator.StringToHash("isGrounded");
  private readonly int FallModifierHash = Animator.StringToHash("FallModifier");

  public bool isGrounded => _isGrounded;
  public bool isRunning => _isRunning;

  public bool isMoving => stateMachine.isMoving;
  [field: SerializeField] public bool RootMotion = true;

  [field: SerializeField] public float WalkSpeed = 1.0f;
  [field: SerializeField] public float RunSpeed = 2.0f;

  [field: SerializeField] public float TurnSpeed = 1.0f;

  public float speed => isRunning? RunSpeed : WalkSpeed;

  public void Awake()
  {
    anim = GetComponent<Animator>();
    if (anim == null) throw new Exception("Animator could not be found");
    anim.applyRootMotion = RootMotion;

    rb = GetComponent<Rigidbody>();
    if (rb == null) throw new Exception("Rigid body could not be found");

    col = GetComponent<CapsuleCollider>();
    if (col == null) throw new Exception("Collider could not be found");

    input = GetComponent<InputReader>();
    if (input == null) throw new Exception("Input Reader could not be found");

    stateMachine = GetComponent<PlayerStateMachine>();
    if (stateMachine == null) throw new Exception("PlayerStateMachine could not be found");

    frontPivot = GameObject.Find("front_pivot");
    backPivot = GameObject.Find("back_pivot");

    EventManager.StartListening<AnimationStateEvent, AnimationStateEventBehavior.AnimationEventType, string>(OnAnimationEvent);
  }

  private void Start()
  {
    Application.targetFrameRate = 60;
    Time.timeScale = 1.0f;
    input.JumpEvent += OnJump;
    input.RunEvent += OnRun;
    input.RunStopEvent += OnRunStop;
  }

  private void OnAnimationEvent(AnimationStateEventBehavior.AnimationEventType eventType, string eventName)
  {
    Debug.Log("EVENT RECEIVED " + eventType + ", " + eventName);
    switch (eventName)
    {
      case AnimationStateEvent.JUMP_COMPLETE:
        if (isGrounded)
        {
          SwitchToMoveState();
        }
        else
        {
          SwitchToFallState();
        }
        break;

      case AnimationStateEvent.LAND_BEGIN:
        float dist = CheckGroundDistance();
        if (dist > 1.5f)
        {
          SwitchToFallState();
        }
        break;

      case AnimationStateEvent.LAND_COMPLETE:
        SwitchToMoveState();
        break;

      default:
        Debug.LogError("Unhandled event: " + eventType + ", " + eventName);
        break;
    }
  }

  private void FixedUpdate()
  {
    _prevGrounded = _isGrounded;
    CheckGrounded();
    if (_isGrounded)
    {
      if (_jump && !_isJumping) {
        SwitchToJumpState();
      }
      if (isMoving) CheckGroundAngle();
    }
    else
    {
      if (_prevGrounded && !_jump && !_isJumping && !_isFalling)
      {
        SwitchToFallState();
      }
      if (_isFalling)
      {
        float distance = CheckGroundDistance();
        float fallModifier = Mathf.Clamp(distance / 10, 0, 1);
        anim.SetFloat(FallModifierHash, fallModifier);
      }
    }
    if (!_isGrounded) Debug.Log("_isGrounded: " + _isGrounded);
  }

  void OnAnimatorMove()
  {
    if (_isJumping)
    {
      anim.ApplyBuiltinRootMotion(); // prefer root motion for jumping animations
      return;
    }

    Vector3 newRootPosition;
    Quaternion newRootRotation;

    if (isGrounded)
    {
      //use root motion as is if on the ground
        newRootPosition = anim.rootPosition;
    }
    else
    {
        //Simple trick to keep model from climbing other rigidbodies that aren't the ground
        newRootPosition = new Vector3(anim.rootPosition.x, this.transform.position.y, anim.rootPosition.z);
    }
    // newRootPosition = anim.rootPosition;
    newRootRotation = anim.rootRotation;
    newRootPosition = Vector3.LerpUnclamped(this.transform.position, newRootPosition, speed);
    float newY = Mathf.Max(0, newRootPosition.y);
    newRootPosition.y = newY;
    newRootRotation = Quaternion.LerpUnclamped(this.transform.rotation, newRootRotation, TurnSpeed);
    rb.MovePosition(newRootPosition);
    rb.MoveRotation(newRootRotation);
  }

  public void Move(Vector3 motion)
  {
    if (isGrounded)
    {
      Vector3 newRootPosition = anim.rootPosition + motion * 1000;
      newRootPosition = Vector3.LerpUnclamped(this.transform.position, newRootPosition, speed);
      rb.MovePosition(newRootPosition);
    }
  }

  public void AddForce(Vector3 motion, ForceMode mode = ForceMode.Impulse)
  {
    rb.AddForce(motion, mode);
  }

  private void SwitchToJumpState()
  {
    _jump = false;
    _isJumping = true;
    _isFalling = false;
    stateMachine.SwitchState(new PlayerJumpState(stateMachine));
  }

  private void SwitchToFallState()
  {
    _isJumping = false;
    _isFalling = true;
    stateMachine.SwitchState(new PlayerFallState(stateMachine));
  }

  private void SwitchToMoveState()
  {
    _isJumping = false;
    _isFalling = false;
    stateMachine.SwitchState(new PlayerMoveState(stateMachine));
  }

  private void CheckGrounded()
  {
    float dist = col.radius + groundCheckTolerance;
    Vector3 pos = rb.transform.position;
    Vector3 origin = new Vector3(pos.x, pos.y + col.radius, pos.z + 0.1f);

    bool hit = RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(origin, Vector3.down, dist, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Both);
    if (hit)
    {
      _isGrounded = true;
      anim.SetBool(isGroundedHash, true);
    }
    else
    {
      _isGrounded = false;
      anim.SetBool(isGroundedHash, false);
    }
  }

  private float CheckGroundDistance()
  {
    Vector3 origin = rb.transform.position;
    origin.y +=  + col.radius;
    RaycastHit hitInfo;
    bool hit = RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(origin, Vector3.down, out hitInfo, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Both);
    if (hit)
    {
      return hitInfo.distance;
    }
    return float.PositiveInfinity;
  }

  private void CheckGroundAngle()
  {
    Vector3 frontOrigin = frontPivot.transform.position;
    Vector3 backOrigin = backPivot.transform.position;
    RaycastHit frontHit;
    RaycastHit backHit;
    RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(frontOrigin, Vector3.down, out frontHit, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Both);
    RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(backOrigin, Vector3.down, out backHit, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Both);
    float deltaHeight = frontHit.distance - backHit.distance;
    Vector3 newVector = (frontOrigin - new Vector3(0, deltaHeight, 0)) - backOrigin;
    RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(backOrigin, newVector, 1.0f, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Both);

    if (Math.Abs(deltaHeight) > 0.01f)
    {
      // Rotate player along up axis
      Quaternion rot = Quaternion.LookRotation(newVector);
      Quaternion newRootRotation = Quaternion.LerpUnclamped(anim.rootRotation, rot, TurnSpeed);
      // Debug.Log("newRootRotation: " + newRootRotation);
      rb.MoveRotation(newRootRotation);
    }
  }

  private void OnJump()
  {
      if (!_isJumping && !_isFalling) _jump = true;
  }

  protected void OnRun()
    {
        _isRunning = true;
        anim.SetBool(isRunningHash, true);
    }

    protected void OnRunStop()
    {
        _isRunning = false;
        anim.SetBool(isRunningHash, false);
    }

  private void OnDestroy()
  {
    input.JumpEvent -= OnJump;
    input.RunEvent -= OnRun;
    input.RunStopEvent -= OnRunStop;
  }
}
