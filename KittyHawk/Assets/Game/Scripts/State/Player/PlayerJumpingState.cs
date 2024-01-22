using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpingState : PlayerMoveBase
{
    private readonly int JumpHash = Animator.StringToHash("Cat_Jump");
    private const float CrossFadeDuration = 0.1f;

    public PlayerJumpingState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        stateMachine.ForceReceiver.Jump(stateMachine.JumpForce);
        stateMachine.Animator.CrossFadeInFixedTime(JumpHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
        Vector3 movement = CalculateMovement();
        Move(movement * stateMachine.FreeMovementSpeed, deltaTime);
        if (stateMachine.Controller.velocity.y <= 0)
        {
            stateMachine.SwitchState(new PlayerFallingState(stateMachine));
        }
        FaceMovementDirection(movement, deltaTime);
    }

    public override void Exit()
    {

    }
}
