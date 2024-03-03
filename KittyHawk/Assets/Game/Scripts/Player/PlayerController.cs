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
  private GameObject frontPivot;
  private GameObject backPivot;
  private bool _isGrounded = true;
  private bool _prevGrounded = true;
  private bool _isJumping = false;
  private bool _jump = false;
  private bool _isFalling = false;
  private bool _isRunning = false;
  private bool _isLanding = false;
  private bool _isAttacking = false;
  private float groundCheckTolerance = 0.1f;
  private readonly int isAttackingHash = Animator.StringToHash("isAttacking");
  private readonly int isRunningHash = Animator.StringToHash("isRunning");
  private readonly int isGroundedHash = Animator.StringToHash("isGrounded");
  private readonly int GroundDistanceHash = Animator.StringToHash("GroundDistance");
  private readonly int FallModifierHash = Animator.StringToHash("FallModifier");
  private Vector3 pendingMotion = Vector3.zero;
  private enum AttackType
  {
    RIGHT,
    LEFT,
    MIDDLE
  }

  public Vector3 velocity => stateMachine.Animator.velocity;
  public bool isAttacking => _isAttacking;
  public bool isGrounded => _isGrounded;
  public bool isRunning => _isRunning;

  public bool isLanding => _isLanding;

  public bool isMoving => stateMachine.isMoving;
  [field: SerializeField] public bool RootMotion = true;

  [field: SerializeField] public float WalkSpeed = 1.0f;
  [field: SerializeField] public float RunSpeed = 2.0f;

  [field: SerializeField] public float BaseJumpForce = 8.0f;

  [field: SerializeField] public float TurnSpeed = 1.0f;

  public float Speed => isRunning? RunSpeed : WalkSpeed;
  public float JumpForce => isRunning? BaseJumpForce * 0.8f : BaseJumpForce;



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
    input.AttackRightEvent += OnAttackRight;
    input.AttackFrontEvent += OnAttackFront;
    input.AttackLeftEvent += OnAttackLeft;
    input.MeowEvent += OnMeow;
  }

  private void OnAnimationEvent(AnimationStateEventBehavior.AnimationEventType eventType, string eventName)
  {
    Debug.Log("EVENT RECEIVED " + eventType + ", " + eventName);
    switch (eventName)
    {
      case AnimationStateEvent.ATTACK_COMPLETE:
        Attack(false);
        break;

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
        if (dist > 3f)
        {
          SwitchToFallState();
        }
        else
        {
          _isLanding = true;
        }
        break;

      case AnimationStateEvent.LAND_COMPLETE:
        SwitchToMoveState();
        break;

      case AnimationStateEvent.MEOW:
        OnMeow();
        break;

      default:
        Debug.LogWarning("Unhandled event: " + eventType + ", " + eventName);
        break;
    }
  }

  private void Update()
  {
    if (anim.updateMode != AnimatorUpdateMode.AnimatePhysics) Execute(Time.deltaTime);
  }

  private void FixedUpdate()
  {
    if (anim.updateMode == AnimatorUpdateMode.AnimatePhysics) Execute(Time.fixedDeltaTime);
  }

  private void Execute(float deltaTime)
  {
    _prevGrounded = _isGrounded;
    float distance = CheckGroundDistance();
    _isGrounded = CheckGrounded() || distance < 0.01f;
    anim.SetBool(isGroundedHash, _isGrounded);
    anim.SetFloat(GroundDistanceHash, distance);

    if (_isGrounded)
    {
      if (_jump && !_isJumping) {
        SwitchToJumpState();
      }
    }
    else
    {
      if (_prevGrounded && !_jump && !_isJumping && !_isFalling)
      {
        SwitchToFallState();
      }
      if (_isFalling)
      {
        float fallModifier = Mathf.Clamp(distance / 7.5f, 0, 1);
        anim.SetFloat(FallModifierHash, fallModifier);
      }
    }
    // if (!_isGrounded) Debug.Log("_isGrounded: " + _isGrounded);
    if (input.MovementValue == Vector2.zero && isMoving)
    {
      SwitchToIdleState();
    }
    Quaternion newRootRotation = GetGroundAngle();
    newRootRotation = Quaternion.LerpUnclamped(this.transform.rotation, newRootRotation, deltaTime);
    rb.MoveRotation(newRootRotation);
  }

  void OnAnimatorMove()
  {
    Vector3 newRootPosition;
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
    newRootPosition = newRootPosition + pendingMotion;
    pendingMotion = Vector3.zero;
    // newRootPosition = anim.rootPosition;
    Quaternion newRootRotation = anim.rootRotation;
    if (isMoving) {
      newRootRotation = GetGroundAngle();
    }

    newRootPosition = Vector3.LerpUnclamped(this.transform.position, newRootPosition, Speed);
    float newY = Mathf.Max(0, newRootPosition.y);
    newRootPosition.y = newY;
    newRootRotation = Quaternion.LerpUnclamped(this.transform.rotation, newRootRotation, TurnSpeed);
    rb.MovePosition(newRootPosition);
    rb.MoveRotation(newRootRotation);
  }

  public void Move(Vector3 motion)
  {
    pendingMotion = motion;
  }

  public void AddForce(Vector3 motion, ForceMode mode = ForceMode.Impulse)
  {
    rb.AddForce(motion, mode);
  }
  public void SwitchToIdleState()
  {
    _isJumping = false;
    _isFalling = false;
    _isLanding = false;
    stateMachine.SwitchState(new PlayerIdleState(stateMachine));
  }

  public void SwitchToMoveState()
  {
    _isJumping = false;
    _isFalling = false;
    _isLanding = false;
    stateMachine.SwitchState(new PlayerMoveState(stateMachine));
  }

  public void SwitchToJumpState()
  {
    _jump = false;
    _isJumping = true;
    _isFalling = false;
    _isLanding = false;
    stateMachine.SwitchState(new PlayerJumpState(stateMachine));
  }

  public void SwitchToFallState()
  {
    _isJumping = false;
    _isFalling = true;
    _isLanding = false;
    stateMachine.SwitchState(new PlayerFallState(stateMachine));
  }

  public bool CheckGrounded()
  {
    Vector3 pos = rb.transform.position;
    if (_isFalling)
    {
      pos = frontPivot.transform.position;
      pos.y -= 0.25f;
    }
    Vector3 origin = new Vector3(pos.x, pos.y + 0.01f, pos.z);
    return RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(origin, Vector3.down, groundCheckTolerance, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor);
  }

  private float CheckGroundDistance()
  {
    Vector3 origin = rb.transform.position;
    origin.y += col.radius;
    RaycastHit hitInfo;
    bool hit = RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(origin, Vector3.down, out hitInfo, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor);
    return hit? hitInfo.distance : float.PositiveInfinity;
  }

  private Quaternion GetGroundAngle()
  {
    Vector3 frontOrigin = frontPivot.transform.position;
    Vector3 backOrigin = backPivot.transform.position;
    RaycastHit frontHit;
    RaycastHit backHit;
    RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(frontOrigin, Vector3.down, out frontHit, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor);
    RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(backOrigin, Vector3.down, out backHit, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor);

    float deltaHeight = frontHit.distance - backHit.distance;
    float clampThreshold = 0.1f;
    if (Math.Abs(deltaHeight) > 0.01f)
    {
      deltaHeight = Mathf.Clamp(deltaHeight, -clampThreshold, clampThreshold);
      Vector3 newVector = (frontOrigin - new Vector3(0, deltaHeight, 0)) - backOrigin;
      RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(backOrigin, newVector, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor);
      // Rotate player along up axis
      Quaternion rot = Quaternion.LookRotation(newVector);
      return Quaternion.LerpUnclamped(anim.rootRotation, rot, TurnSpeed);
    }
    return anim.rootRotation;
  }

  private void Attack(bool b)
  {
    _isAttacking = b;
    anim.SetBool(isAttackingHash, b);
    if (stateMachine.isSitting || stateMachine.isLaying)
    {
      SwitchToIdleState();
    }
  }

  public void Meow()
  {
    Debug.Log("Play meow");
    stateMachine.SwitchAction(new PlayerMeowAction(stateMachine));
  }

  private void OnAttackRight()
  {
    Attack(true);
    stateMachine.SwitchAction(new PlayerAttackAction(stateMachine, PlayerAttackAction.ATTACK_RIGHT));
  }

  private void OnAttackFront()
  {
    Attack(true);
    stateMachine.SwitchAction(new PlayerAttackAction(stateMachine, PlayerAttackAction.ATTACK_FRONT));
  }

  private void OnAttackLeft()
  {
    Attack(true);
    stateMachine.SwitchAction(new PlayerAttackAction(stateMachine, PlayerAttackAction.ATTACK_LEFT));
  }

  private void OnJump()
  {
      if (!_isJumping && !_isFalling) _jump = true;
  }

  private void OnMeow()
  {
    Meow();
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
    input.AttackRightEvent -= OnAttackRight;
    input.AttackFrontEvent -= OnAttackFront;
    input.AttackLeftEvent -= OnAttackLeft;
    input.JumpEvent -= OnJump;
    input.MeowEvent -= OnMeow;
    input.RunEvent -= OnRun;
    input.RunStopEvent -= OnRunStop;
  }
}
