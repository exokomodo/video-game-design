using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerMoveBase
{
    private readonly int AnimStateHash = Animator.StringToHash("JumpingState");
    private float elapsedTime;
    private Vector3 previousVelocity;
    private float prevY;

    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        this.StateID = 4;
    }




    public override void Enter()
    {
        Debug.Log("Enter PlayerJumpState");
        // base.Enter();
        // stateMachine.isJumping = true;
        // stateMachine.Animator.SetInteger(StateIDHash, StateID);
        Debug.Log("PlayerJumpState StateIDHash: " + stateMachine.Animator.GetInteger(StateIDHash));
        // stateMachine.Animator.Play(StateHash);
        stateMachine.ForceReceiver.Jump(stateMachine.JumpForce);
        // stateMachine.Animator.CrossFadeInFixedTime(JumpHash, CrossFadeDuration);
        // stateMachine.Animator.applyRootMotion = true;
        elapsedTime = 0;
        previousVelocity = Vector3.zero;
        prevY = -1;
        // stateMachine.Animator.Play(AnimStateHash); // Begin moving immediately
        // stateMachine.Animator.SetBool("StateChange", true);
        // stateMachine.Animator.SetInteger(Animator.StringToHash("StateID"), StateID);
        // stateMachine.Animator.SetBool("StateChange", true);
    }

    public override void Execute(float deltaTime)
    {
        base.Execute(deltaTime);
        Vector3 movement = CalculateMovement();
        Debug.Log("VelX: " + movement.x + ", VelZ: " + movement.z);
    }

    /*
    public override void Execute(float deltaTime)
    {
        // Debug.Log("StateIDHash: " + stateMachine.Animator.GetInteger(StateIDHash));
        Vector3 movement = CalculateMovement();
        // // Move(movement, deltaTime);
        // Debug.Log("PlayerJumpSate movement:" + movement);
        // stateMachine.Animator.SetFloat(VelocityXHash, movement.x, AnimatorDampTime, deltaTime);
        // stateMachine.Animator.SetFloat(VelocityZHash, movement.z, AnimatorDampTime, deltaTime);
        // FaceMovementDirection(movement, deltaTime);
        // Vector3 movement = CalculateMovement();
        // float speed = stateMachine.isRunning? stateMachine.RunningSpeed : stateMachine.FreeMovementSpeed;
        // Jump(movement * speed, deltaTime);
        // Jump(new Vector3(0, 1, 0) * stateMachine.JumpForce, deltaTime);
        float y = stateMachine.transform.position.y;
        Debug.Log("StateMachine velocity: " + stateMachine.Controller.velocity.y + ", " + y);

        // if (stateMachine.Controller.velocity.y <= 0.01f && y < prevY && y > 0.5f)
        // {
        //     stateMachine.SwitchState(new PlayerFallState(stateMachine));
        // }
        elapsedTime += deltaTime;
        previousVelocity = stateMachine.Controller.velocity;
        prevY = y;
        FaceMovementDirection(movement, deltaTime);
    }
    */

    public override void Exit()
    {
        Debug.Log("Exit PlayerJumpState");
        // base.Exit();
        // stateMachine.isJumping = false;
    }
}
