using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveBase : PlayerBaseState
{
    protected readonly int VelocityXHash = Animator.StringToHash("VelocityX");
    protected readonly int VelocityZHash = Animator.StringToHash("VelocityZ");
    protected const float AnimatorDampTime = 0.1f;
    protected const float CrossFadeDuration = 0.1f;

    public PlayerMoveBase(PlayerStateMachine stateMachine) : base(stateMachine) {}
    public override void Enter()
    {

    }

    public override void Execute(float deltaTime) {
        Vector3 movement = CalculateMovement();
        // FaceMovementDirection(movement, deltaTime);
        stateMachine.Animator.SetFloat(VelocityXHash, movement.x, AnimatorDampTime, deltaTime);
        stateMachine.Animator.SetFloat(VelocityZHash, movement.z, AnimatorDampTime, deltaTime);
        if (stateMachine.InputReader.MovementValue == Vector2.zero && !stateMachine.isJumping)
        {
            // If player is not moving, switch to Idle State
            stateMachine.SwitchState(new PlayerIdleState(stateMachine));
        }

    }

    public override void Exit()
    {

    }

    protected Vector3 CalculateMovement()
    {
        // Vector3 forward = stateMachine.MainCameraTransform.forward;
        // Vector3 right = stateMachine.MainCameraTransform.right;
        // forward.y = 0f;
        // right.y = 0f;
        // forward.Normalize();
        // right.Normalize();
        // return forward * stateMachine.InputReader.MovementValue.y +
        //     right * stateMachine.InputReader.MovementValue.x;
        Vector2 mv = stateMachine.InputReader.MovementValue;
        Debug.Log("isRunning" + stateMachine.isRunning);
        mv = stateMachine.isRunning? mv * stateMachine.Controller.RunSpeed : mv * stateMachine.Controller.WalkSpeed;
        return new Vector3(mv.x, 0, mv.y);
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
