using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSitState : PlayerBaseState
{
    private float timer;

    public int range = 0;

    public static int StateID = 1;

    public PlayerSitState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        Debug.Log("PlayerSit State Enter");
        timer = Mathf.Floor(Random.Range(10, 20));
        stateMachine.Animator.SetInteger(StateIDHash, StateID);


    }

    public override void Execute(float deltaTime)
    {
        timer -= deltaTime;
        if (timer <= 0 ) {
            Debug.Log("Switch to laying down");
            float randomState = Random.Range(0, 2);
            Debug.Log("randomState: " + randomState);
            State newState = randomState < 1? new PlayerIdleState(stateMachine) : new PlayerLieState(stateMachine);
            stateMachine.SwitchState(newState);
        }
    }

    public override void Exit()
    {

    }
}
