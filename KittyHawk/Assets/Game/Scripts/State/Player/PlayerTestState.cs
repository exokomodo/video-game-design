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
        // stateMachine.transform.Translate(movement * deltaTime);
        stateMachine.Controller.Move(movement * stateMachine.FreeMovementSpeed * deltaTime);
        // if (stateMachine.InputReader.MovementValue == Vector2.zero) {
        //     stateMachine.Animator.SetFloat("FreeMoveSpeed", 0, 0.1f, deltaTime);
        // }
        stateMachine.Animator.SetFloat("FreeMoveSpeed", 1, 0.5f, deltaTime);
        stateMachine.transform.rotation = Quaternion.LookRotation(movement);
        // Debug.Log(stateMachine.InputReader.MovementValue);
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
