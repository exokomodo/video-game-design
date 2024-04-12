using System;
using UnityEngine;

/// <summary>
/// Animates Kitty's walking, trotting & running.
/// Author: Geoffrey Roth
/// </summary>
public class PlayerMoveState : PlayerMoveBase
{
    private readonly int MoveHash = Animator.StringToHash("Move");
    private Vector3 delta;

    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        StateID = (int)PlayerStateMachine.StateEnum.MOVE;
    }

    public override void Enter()
    {
        Debug.Log("PlayerMoveState Enter");
        stateMachine.Animator.CrossFadeInFixedTime(MoveHash, CrossFadeDuration);
    }

    public override void Execute(float deltaTime)
    {
        Move(deltaTime);
    }

    protected void Move(float deltaTime) {
        float damp = AnimatorDampTime * stateMachine.Controller.Speed;
        Vector3 prevMovement = new Vector3(stateMachine.Animator.GetFloat(VelocityXHash), 0, stateMachine.Animator.GetFloat(VelocityZHash));
        Vector3 rawMovement = GetNormalizedMovement() * stateMachine.Controller.Speed;
        delta = rawMovement - prevMovement;

        if (delta.magnitude >= 0.8f) {
            // Notable change in movement
            // Debug.Log($"raw: {rawMovement}, delta: {delta}");
            if (delta.z <= -0.8f && rawMovement.z < -0.8f) {
                // Debug.Log($"TURN BACKWARDS 180");
                if (delta.x >= 0) {
                    stateMachine.Controller.SwitchToTurnState(PlayerTurnState.Turn.RIGHT180);
                } else {
                    stateMachine.Controller.SwitchToTurnState(PlayerTurnState.Turn.LEFT180);
                }
                return;
            }
        }

        stateMachine.Animator.SetFloat(VelocityXHash, rawMovement.x, damp, deltaTime);
        stateMachine.Animator.SetFloat(VelocityZHash, rawMovement.z, damp, deltaTime);

        Vector3 heading = CalculateHeading();
        FaceMovementDirection(heading, deltaTime);

        if (Math.Abs(rawMovement.x) < Mathf.Epsilon && Math.Abs(rawMovement.z) < Mathf.Epsilon) {
            stateMachine.SwitchState(new PlayerIdleState(stateMachine));
        }
    }
}
