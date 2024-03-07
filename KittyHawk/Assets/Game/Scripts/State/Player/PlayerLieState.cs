using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The supplemental idle-ish state for Kitty. She may transition here randomly from the sit state.
/// Author: Geoffrey Roth
/// </summary>
public class PlayerLieState : PlayerBaseState
{
    private float timer;

    public int range = 0;

    public PlayerLieState(PlayerStateMachine stateMachine) : base(stateMachine) {
        StateID = (int)PlayerStateMachine.StateEnum.LIE;
    }

    public override void Enter()
    {
        Debug.Log("PlayerLieState Enter");
        timer = Mathf.Floor(Random.Range(5, 15));
    }

    public override void Execute(float deltaTime)
    {
        timer -= deltaTime;
        if (timer <= 0 ) {
            stateMachine.SwitchState(new PlayerSitState(stateMachine));
        }
    }

    public override void Exit()
    {
        Debug.Log("PlayerLieState Exit");
    }
}
