using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveBase : PlayerBaseState
{
    public PlayerMoveBase(PlayerStateMachine stateMachine) : base(stateMachine) {}
    public override void Enter() {}

    public override void Tick(float deltaTime) {}

    public override void Exit() {}

    protected Vector3 CalculateMovement()
    {
        Vector3 forward = stateMachine.MainCameraTransform.forward;
        Vector3 right = stateMachine.MainCameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        return forward * stateMachine.InputReader.MovementValue.y +
            right * stateMachine.InputReader.MovementValue.x;
    }

    protected void FaceMovementDirection(Vector3 movement, float deltaTime)
    {
        stateMachine.transform.rotation = Quaternion.Lerp(
            stateMachine.transform.rotation,
            Quaternion.LookRotation(movement),
            deltaTime * stateMachine.RotationDamping
        );
    }
}
