using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Kitty Idle state
/// Kitty will alternate between standing, sitting and laying down
/// when no controller input is received.
/// Author: Geoffrey Roth
/// </summary>
public class PlayerIdleState : PlayerBaseState
{
    protected readonly int VelocityXHash = Animator.StringToHash("VelocityX");
    protected readonly int VelocityZHash = Animator.StringToHash("VelocityZ");
    protected readonly int isSwimmingHash = Animator.StringToHash("isSwimming");
    protected const float AnimatorDampTime = 0.1f;
    private bool isSwimming;
    private float timer;
    public int range = 0;

    public PlayerIdleState(PlayerStateMachine stateMachine, bool isSwimming = false) : base(stateMachine) {
        StateID = (int)PlayerStateMachine.StateEnum.IDLE;
        this.isSwimming = isSwimming;
    }

    public override void Enter()
    {
        Debug.Log("PlayerIdleState Enter");
        stateMachine.Animator.SetBool(isSwimmingHash, isSwimming);
        resetTimer();
    }

    public override void Execute(float deltaTime)
    {
        stateMachine.Animator.SetFloat(VelocityXHash, 0, AnimatorDampTime, deltaTime);
        stateMachine.Animator.SetFloat(VelocityZHash, 0, AnimatorDampTime, deltaTime);
        if (stateMachine.Controller.isAttacking) {
            resetTimer();
            return;
        };
        timer -= deltaTime;
        if (timer <= 0 && !isSwimming)
        {
            stateMachine.SwitchState(new PlayerSitState(stateMachine));
        }
    }

    public override void Exit()
    {
        Debug.Log("PlayerIdleState Exit");
    }

    private void resetTimer()
    {
        timer = Mathf.Floor(Random.Range(10, 20));
    }
}
