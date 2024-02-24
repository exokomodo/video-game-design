using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpingState : PlayerMoveBase
{
    private readonly int JumpHash = Animator.StringToHash("Cat_Jump");
    private const float CrossFadeDuration = 0.1f;

    public PlayerJumpingState(PlayerStateMachine stateMachine, bool isRunning) : base(stateMachine, isRunning) {}

    public override void Enter()
    {
        stateMachine.ForceReceiver.Jump(stateMachine.JumpForce);
        stateMachine.Animator.CrossFadeInFixedTime(JumpHash, CrossFadeDuration);
    }

    public override void Execute(float deltaTime)
    {
        Vector3 movement = CalculateMovement();
        float speed = isRunning? stateMachine.RunningSpeed : stateMachine.FreeMovementSpeed;
        Move(movement * speed, deltaTime);
        if (stateMachine.Controller.velocity.y <= 0)
        {
            stateMachine.SwitchState(new PlayerFallingState(stateMachine, isRunning));
        }
        FaceMovementDirection(movement, deltaTime);
    }
}
