using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animates Kitty's walking, trotting & running.
/// Author: Geoffrey Roth
/// </summary>
public class PlayerSwimState : PlayerMoveBase
{
    private readonly int AnimHash = Animator.StringToHash("Move");
    private readonly int isSwimmingHash = Animator.StringToHash("isSwimming");

    public PlayerSwimState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        StateID = (int)PlayerStateMachine.StateEnum.MOVE;
    }

    public override void Enter()
    {
        Debug.Log("PlayerSwimState Enter");
        stateMachine.Animator.SetBool(isSwimmingHash, true);
        // stateMachine.transform.position.y = ;
        // stateMachine.Animator.Play(AnimHash); // Begin moving immediately
        stateMachine.Animator.CrossFadeInFixedTime(AnimHash, CrossFadeDuration);
    }

    public override void Execute(float deltaTime)
    {
        // base.Execute(deltaTime);
    }

    public override void Exit()
    {
        // base.Exit();
        // stateMachine.Animator.SetBool(isSwimmingHash, false);
        // stateMachine.transform.Translate(new Vector3(0, 1, 0));
    }
}
