using System;
using UnityEngine;

/// <summary>
/// Animates Kitty's walking, trotting & running.
/// Author: Geoffrey Roth
/// </summary>
public class PlayerMoveState : PlayerMoveBase
{
    private readonly int MoveHash = Animator.StringToHash("Move");

    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        StateID = (int)PlayerStateMachine.StateEnum.MOVE;
    }

    public override void Enter()
    {
        Debug.Log("PlayerMoveState Enter");
        // stateMachine.Animator.Play(MoveHash); // Begin moving immediately
        // stateMachine.Animator.CrossFadeInFixedTime(MoveHash, CrossFadeDuration);
    }

    public override void Execute(float deltaTime)
    {
        Move(deltaTime);
    }

    protected void Move(float deltaTime) {
        float damp = AnimatorDampTime * 1/stateMachine.Controller.Speed;
        Vector3 rawMovement = GetNormalizedMovement() * stateMachine.Controller.Speed;
        Vector3 movement = CalculateHeading();
        FaceMovementDirection(movement, deltaTime);
        stateMachine.Animator.SetFloat(VelocityXHash, rawMovement.x, damp, deltaTime);
        stateMachine.Animator.SetFloat(VelocityZHash, rawMovement.z, damp, deltaTime);
        if (Math.Abs(movement.x) < Mathf.Epsilon && Math.Abs(movement.z) < Mathf.Epsilon) {
            stateMachine.SwitchState(new PlayerIdleState(stateMachine));
        }
    }
}
