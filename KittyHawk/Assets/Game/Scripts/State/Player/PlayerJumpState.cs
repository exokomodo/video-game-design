using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerMoveBase
{
    private readonly int StateHash = Animator.StringToHash("JumpState");
    private readonly int VelocityXHash = Animator.StringToHash("VelocityX");
    private readonly int VelocityZHash = Animator.StringToHash("VelocityZ");
    private const float AnimatorDampTime = 0.1f;

    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine) {
        this.StateID = 4;
    }

    public override void Enter()
    {
        Debug.Log("Enter PlayerJumpState");
        base.Enter();
        // stateMachine.isJumping = true;
        stateMachine.Animator.SetInteger(StateIDHash, StateID);
        Debug.Log("StateIDHash: " + stateMachine.Animator.GetInteger(StateIDHash));
        // stateMachine.Animator.Play(StateHash);
        stateMachine.ForceReceiver.Jump(stateMachine.JumpForce);
        // stateMachine.Animator.CrossFadeInFixedTime(JumpHash, CrossFadeDuration);
        // stateMachine.Animator.applyRootMotion = true;
    }

    public override void Execute(float deltaTime)
    {
        // Vector3 movement = CalculateMovement();
        // // Move(movement, deltaTime);
        // Debug.Log("PlayerJumpSate movement:" + movement);
        // stateMachine.Animator.SetFloat(VelocityXHash, movement.x, AnimatorDampTime, deltaTime);
        // stateMachine.Animator.SetFloat(VelocityZHash, movement.z, AnimatorDampTime, deltaTime);
        // FaceMovementDirection(movement, deltaTime);
    }

    public override void Exit()
    {
        Debug.Log("Exit PlayerJumpState");
        base.Exit();
        stateMachine.isJumping = false;
    }
}
