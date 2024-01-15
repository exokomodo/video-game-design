using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    private float timer = 5.0f;
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        Debug.Log("Enter");
    }

    public override void Tick(float deltaTime)
    {
        Debug.Log("Tick");
        timer -= deltaTime;
        if (timer <= 0 ) {
            stateMachine.SwitchState(new PlayerIdleState(stateMachine));
        }
    }

    public override void Exit()
    {
        Debug.Log("Exit");
    }
}
