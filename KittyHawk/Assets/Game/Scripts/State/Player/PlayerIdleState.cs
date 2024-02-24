using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    private float timer;

    public int range = 0;

    public static int StateID = 0;

    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        timer = Mathf.Floor(Random.Range(10, 20));
        stateMachine.Animator.SetInteger(StateIDHash, StateID);
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

    }
}
