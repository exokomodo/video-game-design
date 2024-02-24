using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLieState : PlayerBaseState
{
    private float timer;

    public int range = 0;

    public static int StateID = 2;

    public PlayerLieState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        Debug.Log("PlayerLie State Enter");
        timer = Mathf.Floor(Random.Range(5, 15));
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
