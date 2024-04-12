using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shared base class for Kitty movement states.
/// Author: Geoffrey Roth
/// </summary>
public class PlayerMoveBase : PlayerBaseState
{
    protected readonly int VelocityXHash = Animator.StringToHash("VelocityX");
    protected readonly int VelocityZHash = Animator.StringToHash("VelocityZ");
    protected const float AnimatorDampTime = 0.05f;
    protected const float CrossFadeDuration = 0.2f;

    public PlayerMoveBase(PlayerStateMachine stateMachine) : base(stateMachine) {}
    public override void Enter()
    {

    }

    public override void Execute(float deltaTime) {
        Vector3 movement = CalculateMovement();
        FaceMovementDirection(movement, deltaTime);
        stateMachine.Animator.SetFloat(VelocityXHash, movement.x, AnimatorDampTime, deltaTime);
        stateMachine.Animator.SetFloat(VelocityZHash, movement.z, AnimatorDampTime, deltaTime);
    }

    public override void Exit()
    {

    }

    protected Vector3 CalculateMovement()
    {
        // Vector2 mv = stateMachine.InputReader.MovementValue;
        // Vector3 movement = new Vector3(mv.x, 0, mv.y);
        // movement.Normalize();
        // return movement;
        return CalculateHeading();
    }

    protected Vector3 GetNormalizedMovement() {
        Vector2 mv = stateMachine.InputReader.MovementValue;
        Vector3 movement = new Vector3(mv.x, 0, mv.y);
        movement.Normalize();
        // Debug.Log($"GetNormalizedMovement: {movement}");
        return movement;
    }

    protected Vector3 CalculateHeading()
    {
        Vector3 forward = stateMachine.MainCameraTransform.forward;
        Vector3 right = stateMachine.MainCameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Debug.Log($"forward: {forward}, right: {right}");
        Vector2 mv = stateMachine.InputReader.MovementValue;
        // Debug.Log($"mv: {mv}");
        Vector3 movement = forward * mv.y + right * mv.x;

        // Debug.Log($"CalculateMovement: {movement}");
        return movement;
    }

    protected void FaceMovementDirection(Vector3 movement, float deltaTime)
    {
        stateMachine.transform.rotation = Quaternion.Lerp(
            stateMachine.transform.rotation,
            Quaternion.LookRotation(movement),
            deltaTime * stateMachine.Controller.RotationDamping
        );
    }


}
