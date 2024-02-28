using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerMoveBase
{
    private readonly int MoveHash = Animator.StringToHash("Move");
    private readonly int VelocityXHash = Animator.StringToHash("VelocityX");
    private readonly int VelocityZHash = Animator.StringToHash("VelocityZ");
    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.1f;


    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        this.StateID = 3;
    }

    public override void Enter()
    {
        Debug.Log("PlayerMoveState Enter");
        base.Enter();
        // stateMachine.Animator.SetInteger(StateIDHash, StateID);
        stateMachine.Animator.SetFloat(VelocityXHash, 0f);
        stateMachine.Animator.SetFloat(VelocityZHash, 0f);
        // stateMachine.Animator.Play(MoveHash);
    }

    public override void Execute(float deltaTime)
    {
        Debug.Log("PlayerMoveState Execute");
        Vector3 movement = CalculateMovement();
        // Move(movement, deltaTime);
        // Debug.Log("movement:" + movement);
        if (stateMachine.InputReader.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(VelocityXHash, 0, AnimatorDampTime, deltaTime);
            stateMachine.Animator.SetFloat(VelocityZHash, 0, AnimatorDampTime, deltaTime);
            // If player is not moving, switch to Idle State
            stateMachine.SwitchState(new PlayerIdleState(stateMachine));
            return;
        }
        stateMachine.Animator.SetFloat(VelocityXHash, movement.x, AnimatorDampTime, deltaTime);
        stateMachine.Animator.SetFloat(VelocityZHash, movement.z, AnimatorDampTime, deltaTime);
        FaceMovementDirection(movement, deltaTime);
    }


}
