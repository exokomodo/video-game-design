using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTestState : PlayerBaseState
{
    public PlayerTestState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        Debug.Log("Enter");
        stateMachine.InputReader.JumpEvent += OnJump;
    }

    public override void Tick(float deltaTime)
    {
        // Debug.Log("Tick");
        Vector3 movement = new Vector3();
        movement.x = stateMachine.InputReader.MovementValue.x;
        movement.y = 0;
        movement.z = stateMachine.InputReader.MovementValue.y;

        Vector3 dmove = movement * stateMachine.FreeMovementSpeed * deltaTime;
        Debug.Log("-------------");
        Debug.Log(dmove.x);
        Debug.Log(dmove.y);
        Debug.Log(dmove.z);
        stateMachine.Controller.Move(dmove);
        // Debug.Log(stateMachine.InputReader.MovementValue);
        if (stateMachine.InputReader.MovementValue == Vector2.zero) {
            stateMachine.Animator.SetFloat("FreeMoveSpeed", 0, 0.1f, deltaTime);
        }
        stateMachine.Animator.SetFloat("FreeMoveSpeed", 1, 0.1f, deltaTime);
        stateMachine.transform.rotation = Quaternion.LookRotation(movement);
    }

    public override void Exit()
    {
        Debug.Log("Exit");
        stateMachine.InputReader.JumpEvent -= OnJump;
    }

    private void OnJump()
    {
        Debug.Log("ON JUMP");
    }
}
