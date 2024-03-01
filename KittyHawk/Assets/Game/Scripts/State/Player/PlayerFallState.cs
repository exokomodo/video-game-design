using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerMoveBase
{
    private Vector3 momentum;

    public PlayerFallState(PlayerStateMachine stateMachine) : base(stateMachine) {
        StateID = 5;
    }

    public override void Enter()
    {
        Debug.Log("PlayerFallState Enter");
        momentum = stateMachine.Controller.velocity;
        momentum.y = 0;
    }

    public override void Execute(float deltaTime)
    {
        AddForce(momentum * deltaTime, ForceMode.VelocityChange);
    }

    public override void Exit()
    {
        Debug.Log("PlayerFallState Exit");
    }
}
