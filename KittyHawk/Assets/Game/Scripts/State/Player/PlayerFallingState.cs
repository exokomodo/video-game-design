using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallingState : PlayerMoveBase
{
    private readonly int FallHash = Animator.StringToHash("Cat_Fall");

    private Vector3 momentum;
    private const float CrossFadeDuration = 0.1f;

    public PlayerFallingState(PlayerStateMachine stateMachine, bool isRunning) : base(stateMachine, isRunning) {}

    public override void Enter()
    {
        momentum = stateMachine.Controller.velocity;
        momentum.y = 0;
        stateMachine.Animator.CrossFadeInFixedTime(FallHash, CrossFadeDuration);
    }

    public override void Execute(float deltaTime)
    {
        Move(momentum, deltaTime);

        if (stateMachine.Controller.isGrounded)
        {
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine, isRunning));
        }
    }
}
