using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerMoveBase
{
    private float elapsedTime;
    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        this.StateID = 4;
        elapsedTime = 0;
    }

    public override void Enter()
    {
        Debug.Log("PlayerJumpState Enter");
    }

    public override void Execute(float deltaTime) {
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
