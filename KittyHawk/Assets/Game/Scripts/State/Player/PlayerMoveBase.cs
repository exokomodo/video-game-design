using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveBase : PlayerBaseState
{
    public PlayerMoveBase(PlayerStateMachine stateMachine, bool isRunning = false) : base(stateMachine)
    {
        this.isRunning = isRunning;
    }
    public override void Enter()
    {
        stateMachine.InputReader.RunEvent += OnRun;
        stateMachine.InputReader.RunStopEvent += OnRunStop;
    }

    public override void Execute(float deltaTime) {}

    public override void Exit()
    {
        stateMachine.InputReader.RunEvent -= OnRun;
        stateMachine.InputReader.RunStopEvent -= OnRunStop;
    }

    protected bool isRunning = false;

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

    protected void OnRun()
    {
        isRunning = true;
    }

    protected void OnRunStop()
    {
        isRunning = false;
    }
}
