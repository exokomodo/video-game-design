using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {
  private Animator anim;
  private Rigidbody rb;
  private InputReader input;
  private PlayerStateMachine stateMachine;
  private CapsuleCollider col;
  public Vector3 velocity => rb.velocity;
  private bool _isGrounded = true;
  private bool _prevGrounded = true;
  private bool _isJumping = false;
  private bool _jump = false;
  private float groundCheckDistance = 0.01f;

  public bool isGrounded => _isGrounded;

  public bool RootMotion = true;

  public void Awake()
  {
    anim = GetComponent<Animator>();
    if (anim == null) throw new Exception("Animator could not be found");

    rb = GetComponent<Rigidbody>();
    if (rb == null) throw new Exception("Rigid body could not be found");

    col = GetComponent<CapsuleCollider>();
    Debug.Log("col" + col.height + ", " + col.radius);
    if (col == null) throw new Exception("Collider could not be found");

    input = GetComponent<InputReader>();
    if (input == null) throw new Exception("Input Reader could not be found");

    stateMachine = GetComponent<PlayerStateMachine>();
    if (stateMachine == null) throw new Exception("PlayerStateMachine could not be found");

    EventManager.StartListening<AnimationStateEvent, AnimationStateEventBehavior.AnimationEventType, string>(OnAnimationEvent);
  }

  private void OnAnimationEvent(AnimationStateEventBehavior.AnimationEventType eventType, string eventName)
  {
    Debug.Log("EVENT RECEIVED " + eventType + ", " + eventName);
  }

  private void Start()
  {
    Application.targetFrameRate = 60;
    Time.timeScale = 1;
    input.JumpEvent += OnJump;
    anim.applyRootMotion = RootMotion;
  }

  private void FixedUpdate()
  {
    CheckGrounded();
    if (_isGrounded)
    {
      if (_jump && !_isJumping) {
        _isJumping = true;
        // anim.applyRootMotion = false;
        stateMachine.SwitchState(new PlayerJumpState(stateMachine));
      }
      if (!_isJumping && !_prevGrounded)
      {
        // anim.applyRootMotion = true;
        stateMachine.SwitchState(new PlayerMoveState(stateMachine));
      }
    }
    else{

    }
    // anim.applyRootMotion = _isGrounded;
    _jump = false;
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
    _prevGrounded = _isGrounded;
    float dist = col.height/2f + groundCheckDistance;
    Vector3 origin = transform.position;
    origin.y += col.radius;

    bool hit = RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(origin, Vector3.down, dist, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Both);
    if (hit)
    {
      _isGrounded = true;
    }
    else
    {
      _isGrounded = false;
    }
    if (_prevGrounded && _isGrounded && _isJumping)
    {
      _isJumping = false;
    }
    // _isGrounded = true;
  }

  private void OnJump()
  {
      if (!_jump) _jump = true;
  }

  private void OnDestroy()
  {
    input.JumpEvent -= OnJump;
  }
}
