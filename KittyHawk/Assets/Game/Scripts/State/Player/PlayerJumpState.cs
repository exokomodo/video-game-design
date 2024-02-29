using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerMoveBase
{
    private Vector3 momentum;

    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        this.StateID = 4;
    }

    public override void Enter()
    {
        Debug.Log("PlayerJumpState Enter");
        base.Enter();
        momentum = stateMachine.Controller.velocity;
        momentum.y = 0;
        // stateMachine.Controller.Jump(new Vector3(0, stateMachine.JumpForce, 0));
        // stateMachine.Animator.CrossFadeInFixedTime(JumpHash, CrossFadeDuration);
        // elapsedTime = 0;
        // previousVelocity = Vector3.zero;
        // prevY = -1;
    }

    public override void Execute(float deltaTime)
    {
        // Override base state
        // Jump(momentum, deltaTime);
        // Move(momentum, deltaTime);
    }

    /*
    public override void Execute(float deltaTime)
    {
        // Jump(movement * speed, deltaTime);
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
        Debug.Log("PlayerJumpState Exit");
    }
}
