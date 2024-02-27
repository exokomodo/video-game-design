using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    private readonly int AnimHash = Animator.StringToHash("Idle");
    private float timer;
    private bool runTimer;

    public int range = 0;

    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) {
        this.StateID = 0;
        runTimer = true;
    }

    public override void Enter()
    {
        Debug.Log("PlayerIdleState Enter");
        timer = Mathf.Floor(Random.Range(10, 20));
        stateMachine.Animator.SetInteger(StateIDHash, StateID);
        stateMachine.Animator.applyRootMotion = false;
    }

    public override void Execute(float deltaTime)
    {
        timer -= deltaTime;
        if (timer <= 0 && runTimer)
        {
            runTimer = false;
            stateMachine.SwitchState(new PlayerSitState(stateMachine));
        }
    }

    public override void Exit()
    {
        Debug.Log("PlayerIdleState Exit");
    }
}
