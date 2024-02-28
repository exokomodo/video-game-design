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
  public Vector3 velocity => rb.velocity;

  private GameObject frontPivot;
  private GameObject backPivot;
  private bool _isGrounded = true;
  private bool _prevGrounded = true;
  private bool _isJumping = false;
  private bool _jump = false;
  private bool _fall = false;
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

  public void Awake()
  {
    anim = GetComponent<Animator>();
    if (anim == null) throw new Exception("Animator could not be found");
    anim.applyRootMotion = RootMotion;

    rb = GetComponent<Rigidbody>();
    if (rb == null) throw new Exception("Rigid body could not be found");

    col = GetComponent<CapsuleCollider>();
    Debug.Log("col" + col.height + ", " + col.radius);
    if (col == null) throw new Exception("Collider could not be found");

    input = GetComponent<InputReader>();
    if (input == null) throw new Exception("Input Reader could not be found");

    stateMachine = GetComponent<PlayerStateMachine>();
    if (stateMachine == null) throw new Exception("PlayerStateMachine could not be found");

    frontPivot = GameObject.Find("front_pivot");
    backPivot = GameObject.Find("back_pivot");
    Debug.Log("frontPivot: " + frontPivot);
    Debug.Log("backPivot: " + backPivot);

    EventManager.StartListening<AnimationStateEvent, AnimationStateEventBehavior.AnimationEventType, string>(OnAnimationEvent);
  }

  private void OnAnimationEvent(AnimationStateEventBehavior.AnimationEventType eventType, string eventName)
  {
    Debug.Log("EVENT RECEIVED " + eventType + ", " + eventName);
    switch (eventName)
    {
      case AnimationStateEvent.LAND_COMPLETE:

        stateMachine.SwitchState(new PlayerIdleState(stateMachine));
        break;

      default:
        Debug.LogError("Unhandled event: " + eventType);
        break;
    }
  }

  private void Start()
  {
    Application.targetFrameRate = 60;
    Time.timeScale = 1.0f;
    input.JumpEvent += OnJump;
    input.RunEvent += OnRun;
    input.RunStopEvent += OnRunStop;
  }

  private void FixedUpdate()
  {
    _prevGrounded = _isGrounded;
    CheckGrounded();
    if (_isGrounded)
    {
      if (_prevGrounded && _isGrounded && _isJumping) {
        _isJumping = false;
      }
      if (_jump && !_isJumping) {
        _isJumping = true;
        // anim.applyRootMotion = false;
        stateMachine.SwitchState(new PlayerJumpState(stateMachine));
      }
      if (!_isFalling && !_isJumping && !_prevGrounded)
      {
        // anim.applyRootMotion = true;
        stateMachine.SwitchState(new PlayerIdleState(stateMachine));
      }
      if (isMoving) CheckGroundAngle();

    }
    else
    {
      if (!_isJumping && !_fall && !_isFalling)
      {
        // _fall = true;
        // _isFalling = true;
        // stateMachine.SwitchState(new PlayerFallState(stateMachine));
      }
    }

    if (_isFalling)
    {
      float distance = CheckGroundDistance();
      float fallModifier = Mathf.Clamp(distance / 10, 0, 1);
      anim.SetFloat(FallModifierHash, fallModifier);
    }

    // anim.applyRootMotion = _isGrounded;
    _jump = false;
    _fall = false;
    if (!_isGrounded) Debug.Log("_isGrounded: " + _isGrounded);
  }

  // If OnAnimatorMove is present, root motion does not automatically change the player position
  // This is leading to excessive camera shake
  /*
  void OnAnimatorMove()
  {
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
    newRootPosition = anim.rootPosition;
    newRootRotation = anim.rootRotation;
    newRootPosition = Vector3.LerpUnclamped(this.transform.position, newRootPosition, stateMachine.FreeMovementSpeed);
    newRootRotation = Quaternion.LerpUnclamped(this.transform.rotation, newRootRotation, 1);
    rb.MovePosition(newRootPosition);
    rb.MoveRotation(newRootRotation);
  }
  */
  public void Move(Vector3 motion)
  {
    // transform.position += motion;
    // rb.AddForce(motion);
    // newRootPosition = Vector3.LerpUnclamped(this.transform.position, newRootPosition, rootMovementSpeed);
    Vector3 newRootPosition = anim.rootPosition;
    newRootPosition = Vector3.LerpUnclamped(this.transform.position, newRootPosition, stateMachine.JumpForce);
    rb.MovePosition(newRootPosition + motion);
  }

  public void Jump(Vector3 motion)
  {
    // transform.position += motion;
    // rb.AddForce(motion);
    // newRootPosition = Vector3.LerpUnclamped(this.transform.position, newRootPosition, rootMovementSpeed);
    // Vector3 newRootPosition = anim.rootPosition;
    // newRootPosition = Vector3.LerpUnclamped(this.transform.position, newRootPosition, stateMachine.JumpForce);
    // rb.MovePosition(newRootPosition + motion);
    // rb.AddForce(new Vector3(0, 1, 0), ForceMode.Impulse);
  }

  private void CheckGrounded()
  {
    // _prevGrounded = _isGrounded;
    float dist = col.height/2f + groundCheckTolerance;
    Vector3 pos = transform.position;
    Vector3 origin = new Vector3(pos.x, pos.y, pos.z);
    origin.y += col.radius;
    origin.z += 0.1f;

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

    if (_prevGrounded && _isGrounded && _isJumping)
    {
      // _isJumping = false;
    }
  }

  private float CheckGroundDistance()
  {
    Vector3 origin = anim.rootPosition;
    RaycastHit hitInfo;
    bool hit = RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(origin, Vector3.down, out hitInfo, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.None);
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
      Debug.Log("newRootRotation: " + newRootRotation);
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
