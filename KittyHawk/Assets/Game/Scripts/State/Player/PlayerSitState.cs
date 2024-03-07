using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A state that Kitty will transition to at random when
/// the player has been idle for some time.
/// Author: Geoffrey Roth
/// </summary>
public class PlayerSitState : PlayerBaseState
{
    private float timer;

    public int range = 0;

    public PlayerSitState(PlayerStateMachine stateMachine) : base(stateMachine) {
        StateID = (int)PlayerStateMachine.StateEnum.SIT;
    }

    public override void Enter()
    {
        Debug.Log("PlayerSitState Enter");
        timer = Mathf.Floor(Random.Range(10, 20));
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
        Debug.Log("PlayerSitState Exit");
    }
}
