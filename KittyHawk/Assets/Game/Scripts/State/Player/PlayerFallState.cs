using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerMoveBase
{
    private readonly int AnimHash = Animator.StringToHash("Fall");
    private Vector3 momentum;

    public PlayerFallState(PlayerStateMachine stateMachine) : base(stateMachine) {
        StateID = 5;
    }

    public override void Enter()
    {
        Debug.Log("PlayerFallState Enter");
        // momentum = stateMachine.Controller.velocity;
        // momentum.y = 0;
        // stateMachine.Animator.CrossFadeInFixedTime(AnimHash, CrossFadeDuration);
    }

    public override void Execute(float deltaTime)
    {
        // Debug.Log("PlayerFallState Execute");
        // Move(momentum, deltaTime);

        if (stateMachine.Controller.isGrounded)
        {
            stateMachine.SwitchState(new PlayerMoveState(stateMachine));
        }
    }

    public override void Exit()
    {
        Debug.Log("PlayerFallState Exit");
    }
}
