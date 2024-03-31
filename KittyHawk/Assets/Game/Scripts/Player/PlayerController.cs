using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// A Player Controller that manages the Kitty Hawk Player model, finite state machine,
/// animation controller, events, and input controls.
/// Author: Geoffrey Roth
/// </summary>
public class PlayerController : MonoBehaviour {
  private Animator anim;
  public Rigidbody rb;
  private InputReader input;
  private PlayerStateMachine stateMachine;
  private CapsuleCollider col;
  private SphereCollider headCol;
  private BoxCollider attackCol;
  private GameObject frontPivot;
  private GameObject backPivot;
  private PlayerInventory inventory;
  private bool _isGrounded = true;
  private bool _prevGrounded = true;
  private bool _isJumping = false;
  private bool _jump = false;
  private bool _isFalling = false;
  private bool _isRunning = false;
  private bool _isLanding = false;
  private bool _isAttacking = false;
  private bool _isActive = false;
  private bool _isDead = false;
  private float groundCheckTolerance = 0.1f;
  private readonly int isAttackingHash = Animator.StringToHash("isAttacking");
  private readonly int isRunningHash = Animator.StringToHash("isRunning");
  private readonly int isGroundedHash = Animator.StringToHash("isGrounded");
  private readonly int GroundDistanceHash = Animator.StringToHash("GroundDistance");
  private readonly int FallModifierHash = Animator.StringToHash("FallModifier");
  private Vector3 pendingMotion = Vector3.zero;
  private Quaternion pendingRotation = Quaternion.identity;
  private float hitTimer;
  private enum AttackType
  {
    RIGHT,
    LEFT,
    MIDDLE
  }
  private AudioSource _walkAudio;

  public Vector3 velocity => stateMachine.Animator.velocity;
  public bool isAttacking => _isAttacking;
  public bool isGrounded => _isGrounded;
  public bool isRunning => _isRunning;
  public bool isLanding => _isLanding;
  public bool isMoving => stateMachine.isMoving;
  public bool isActive {
    get { return _isActive; }
    private set {
      _isActive = value;
    }
  }
  public bool isDialogOpen { get; private set; }
  public Vector3 headPosition => new Vector3(headCol.transform.position.x + headCol.radius, headCol.transform.position.y, headCol.transform.position.z);
  public float Speed => isRunning? RunSpeed : WalkSpeed;
  public float JumpForce => isRunning? BaseJumpForce * 0.8f : BaseJumpForce;
  public AudioClip WalkClip;
  public AudioClip RunClip;


  public bool RootMotion = true;
  public float WalkSpeed = 1.0f;
  public float RunSpeed = 2.0f;
  public float BaseJumpForce = 8.0f;
  public float TurnSpeed = 1.0f;
  public float HitCooldown = 2.0f;


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

    headCol = GetComponentInChildren<SphereCollider>();
    if (headCol == null) throw new Exception("Collider could not be found");

    inventory = GetComponent<PlayerInventory>();
    if (inventory == null) throw new Exception("PlayerInventory component could not be found");

    attackCol = GetComponent<BoxCollider>();

    frontPivot = GameObject.Find("front_pivot");
    backPivot = GameObject.Find("back_pivot");

    _walkAudio = GetComponent<AudioSource>();

    EventManager.StartListening<AnimationStateEvent, AnimationStateEventBehavior.AnimationEventType, string>(OnAnimationEvent);
    EventManager.StartListening<DialogueOpenEvent, Vector3, string>(OnDialogOpen);
    EventManager.StartListening<DialogueCloseEvent, string>(OnDialogClose);
    EventManager.StartListening<InteractionEvent, string, string, InteractionTarget>(OnInteractionEvent);
    EventManager.StartListening<VolumeChangeEvent, float>(VolumeChangeHandler);
  }

  private void Start()
  {
    Application.targetFrameRate = 60;
    Time.timeScale = 1.0f;
    ToggleActive(true);
    hitTimer = HitCooldown;
  }

  private void UpdateAudio()
  {
    _walkAudio.clip = isRunning? RunClip : WalkClip;
    if (isMoving && !_walkAudio.isPlaying)
    {
        _walkAudio.Play();
    }
    else if (!isMoving && _walkAudio.isPlaying)
    {
        _walkAudio.Stop();
    }
  }

  private void VolumeChangeHandler(float volume)
  {
      _walkAudio.volume = volume;
  }

  private void ResetHitTimer()
  {
    hitTimer = 0;
  }

  void OnCollisionEnter(Collision collision)
  {
    if (collision.transform.gameObject.tag == "Goose")
    {
      if (hitTimer > HitCooldown)
      {
        ResetHitTimer();
        if (--inventory.Lives > 0)
        {
          stateMachine.SwitchAction(new PlayerHitAction(stateMachine));
        }
        else
        {
          _isDead = true;
          stateMachine.SwitchState(new PlayerDieState(stateMachine));
          ToggleActive(false);
        }

      }
    }
  }

  void OnTriggerEnter(Collider c)
  {
    if (c.CompareTag("Bunny")) {
      Debug.Log("OnTriggerEnter Doorway");
      EventManager.TriggerEvent<LevelEvent<Collider>, string, Collider>(LevelEvent<Room>.END_ROOM_ENTERED, c);
      return;
    }
    if (_isAttacking)
    {
      Debug.Log($"HIT Collider {c}");
      EventManager.TriggerEvent<AttackEvent, string, float, Collider>(AttackEvent.ATTACK_TARGET_HIT, 0f, c);
    }
  }

  private void OnInteractionEvent(string eventName, string eventType, InteractionTarget target)
  {
    Debug.Log("InteractionEvent received " + eventName + ", " + eventType);
    switch (eventName)
    {
      case InteractionEvent.INTERACTION_ZONE_ENTERED:
        // TODO: Notify player that an interaction is available
        break;

      case InteractionEvent.INTERACTION_ZONE_EXITED:
        // TODO: Remove interaction availability
        break;

      case InteractionEvent.INTERACTION_TRIGGERED:
        if (eventType != InteractionType.NONE)
          SwitchToInteractState(eventType, target);
        break;

      default:
        Debug.LogWarning("Unhandled InteractionEvent: " + eventName + ", " + eventType);
        break;
    }
  }

  private void OnAttackEvent(string eventType, float attackTime, Collider c)
  {
    switch (eventType)
    {
      case AttackEvent.ATTACK_BEGIN:
        Attack(true);
        break;
      case AttackEvent.ATTACK_END:
        Attack(false);
        break;
    }
  }

  private void OnAnimationEvent(AnimationStateEventBehavior.AnimationEventType eventType, string eventName)
  {
    Debug.Log("AnimationEvent received " + eventType + ", " + eventName);
    switch (eventName)
    {
      case AnimationStateEvent.INTERACTION_COMPLETE:
        SwitchToIdleState();
        ToggleActive(true);
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
    hitTimer += deltaTime;
    _prevGrounded = _isGrounded;
    float distance = CheckGroundDistance();
    _isGrounded = CheckGrounded() || distance < 0.01f;
    anim.SetBool(isGroundedHash, _isGrounded);
    anim.SetFloat(GroundDistanceHash, distance);

    if (_isGrounded)
    {
      if (_jump && !_isJumping && !_isFalling) {
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
    if (input.MovementValue == Vector2.zero && isMoving && !_isFalling && !_isDead)
    {
      SwitchToIdleState();
    }
    UpdateAudio();
  }

  void OnAnimatorMove()
  {
    if (isDialogOpen) return;

    Vector3 newRootPosition;
    if (isGrounded)
    {
      // use root motion when grounded
      newRootPosition = anim.rootPosition;
    }
    else
    {
        // Simple trick to keep model from climbing other rigidbodies that aren't the ground
        newRootPosition = new Vector3(anim.rootPosition.x, this.transform.position.y, anim.rootPosition.z);
    }
    newRootPosition = newRootPosition + pendingMotion;
    pendingMotion = Vector3.zero;
    newRootPosition = Vector3.LerpUnclamped(this.transform.position, newRootPosition, Speed);
    float newY = Mathf.Max(0, newRootPosition.y);
    newRootPosition.y = newY;

    Quaternion newRootRotation;
    if (pendingRotation != Quaternion.identity)
    {
      newRootRotation = pendingRotation;
      pendingRotation = Quaternion.identity;
    }
    else
    {
      newRootRotation = isMoving? GetGroundAngle() : anim.rootRotation;
      newRootRotation = Quaternion.LerpUnclamped(this.transform.rotation, newRootRotation, TurnSpeed);
    }

    rb.MovePosition(newRootPosition);
    rb.MoveRotation(newRootRotation);
  }

  public void Enable()
  {
    ToggleActive(true);
    ToggleListeners(true);
    SwitchToIdleState();
  }

  public void Disable()
  {
    ToggleActive(false);
    ToggleRunning(false);
    ToggleListeners(false);
  }

  public void ToggleActive(bool b)
  {
    ToggleListeners(b);
    isActive = b;
    if (!b && !_isDead && stateMachine.CurrentStateID != (int)PlayerStateMachine.StateEnum.IDLE)
    {
      SwitchToIdleState();
    }
  }

  public void ToggleRunning(bool b)
  {
    _isRunning = b;
    anim.SetBool(isRunningHash, b);

  }

  public void Move(Vector3 motion)
  {
    pendingMotion = motion;
  }

  public void Rotate(Quaternion rotation)
  {
    pendingRotation = rotation;
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

  public void SwitchToInteractState(string eventType, InteractionTarget target)
  {
    _isJumping = false;
    _isFalling = false;
    _isLanding = false;
    ToggleListeners(false);
    isActive = false;
    stateMachine.SwitchState(new PlayerInteractState(stateMachine, eventType, target));
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
    Vector3 origin = anim.rootPosition;
    Quaternion rot = anim.rootRotation;
    Quaternion newRotation;

    RaycastHit frontHit;
    RaycastHit backHit;
    RaycastHit hit;
    RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(frontOrigin, Vector3.down, out frontHit, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor);
    RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(backOrigin, Vector3.down, out backHit, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor);
    RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(origin, Vector3.down, out hit, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor);

    float deltaHeight = frontHit.distance - backHit.distance;
    float clampThreshold = 1.0f;
    float angle = Vector3.Angle(hit.normal, Vector3.up);
    if (Math.Abs(deltaHeight) > 0.01f)
    {
      Vector3 newVector;
      if (Math.Abs(angle) > 0.01f)
      {
        deltaHeight = Mathf.Clamp(deltaHeight, -clampThreshold, clampThreshold);
        newVector = frontOrigin - new Vector3(0, deltaHeight, 0) - backOrigin;
      }
      else
      {
        newVector = frontOrigin - backOrigin;
        newVector.y = 0;

      }
      newRotation = Quaternion.LookRotation(newVector);
      return Quaternion.LerpUnclamped(rot, newRotation, Time.fixedDeltaTime * TurnSpeed);
    }
    return rot;
  }

  public void Attack(bool b)
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
    stateMachine.SwitchAction(new PlayerMeowAction(stateMachine));
  }

  private void OnAttackRight()
  {
    Attack(true);
    stateMachine.SwitchAction(new PlayerAttackAction(stateMachine, (int)PlayerStateMachine.ActionEnum.ATTACK_RIGHT));
  }

  private void OnAttackFront()
  {
    Attack(true);
    stateMachine.SwitchAction(new PlayerAttackAction(stateMachine, (int)PlayerStateMachine.ActionEnum.ATTACK_FRONT));
  }

  private void OnAttackLeft()
  {
    Attack(true);
    stateMachine.SwitchAction(new PlayerAttackAction(stateMachine, (int)PlayerStateMachine.ActionEnum.ATTACK_LEFT));
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
      ToggleRunning(true);
  }

  protected void OnRunStop()
  {
      ToggleRunning(false);
  }

  private void ToggleListeners(bool b)
  {
    input.AttackRightEvent -= OnAttackRight;
    input.AttackFrontEvent -= OnAttackFront;
    input.AttackLeftEvent -= OnAttackLeft;
    input.JumpEvent -= OnJump;
    input.MeowEvent -= OnMeow;
    input.RunEvent -= OnRun;
    input.RunStopEvent -= OnRunStop;
    if (b) {
      input.JumpEvent += OnJump;
      input.RunEvent += OnRun;
      input.RunStopEvent += OnRunStop;
      input.AttackRightEvent += OnAttackRight;
      input.AttackFrontEvent += OnAttackFront;
      input.AttackLeftEvent += OnAttackLeft;
      input.MeowEvent += OnMeow;
    }
  }

  private void OnDialogOpen(Vector3 position, string dialogueName)
  {
    ToggleDialogOpen(true);
  }

  private void OnDialogClose(string dialogueName)
  {
    ToggleDialogOpen(false);
  }

  private void ToggleDialogOpen(bool b)
  {
    ToggleActive(!b);
    isDialogOpen = b;
  }

  private void OnDestroy()
  {
    ToggleListeners(false);
    EventManager.StopListening<AnimationStateEvent, AnimationStateEventBehavior.AnimationEventType, string>(OnAnimationEvent);
    EventManager.StopListening<DialogueOpenEvent, Vector3, string>(OnDialogOpen);
    EventManager.StopListening<DialogueCloseEvent, string>(OnDialogClose);
    EventManager.StopListening<InteractionEvent, string, string, InteractionTarget>(OnInteractionEvent);
    EventManager.StopListening<VolumeChangeEvent, float>(VolumeChangeHandler);
  }
}
