using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunningState : PlayerBaseState
{
    private readonly int FallHash = Animator.StringToHash("Cat|Run_Forward");

    private Vector3 momentum;
    private const float CrossFadeDuration = 0.1f;

    public PlayerRunningState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        momentum = stateMachine.Controller.velocity;
        momentum.y = 0;
        stateMachine.Animator.CrossFadeInFixedTime(FallHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
        Move(momentum, deltaTime);

        if (stateMachine.Controller.isGrounded)
        {
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
        }
    }

    public override void Exit()
    {

    }
}
