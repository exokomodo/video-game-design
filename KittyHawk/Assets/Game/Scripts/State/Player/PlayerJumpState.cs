using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerMoveBase
{
    private float elapsedTime = 0;
    private bool jumpForceApplied = false;

    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        StateID = (int)PlayerStateMachine.StateEnum.JUMP;
    }

    public override void Enter()
    {
        Debug.Log("PlayerJumpState Enter");
    }

    public override void Execute(float deltaTime)
    {
        // Delay jump force to allow for pre-jump crouch animation
        if (!jumpForceApplied && elapsedTime > 0.2)
        {
            float jumpForce = stateMachine.Controller.JumpForce;
            stateMachine.ForceReceiver.Jump(jumpForce);
            jumpForceApplied = true;
        }

        // Allow player to move in air a bit
        if (jumpForceApplied)
        {
            Vector3 movement = CalculateRelativeMovement();
            Move(movement, deltaTime);
        }

        elapsedTime += deltaTime;
        if (elapsedTime > 2f)
        {
            // Sometimes KH gets stuck in the jump state
            // This will help her transition back to a moveable state
            EventManager.TriggerEvent<AnimationStateEvent, AnimationStateEventBehavior.AnimationEventType, string>(AnimationStateEventBehavior.AnimationEventType.TIME, AnimationStateEvent.JUMP_COMPLETE);
        }
    }

    public override void Exit()
    {
        Debug.Log("PlayerJumpState Exit");
    }
}
