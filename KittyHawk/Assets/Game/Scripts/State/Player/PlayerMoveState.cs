using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animates Kitty's walking, trotting & running.
/// Author: Geoffrey Roth
/// </summary>
public class PlayerMoveState : PlayerMoveBase
{
    private readonly int MoveHash = Animator.StringToHash("Move");

    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        StateID = (int)PlayerStateMachine.StateEnum.MOVE;
    }

    public override void Enter()
    {
        Debug.Log("PlayerMoveState Enter");
        base.Enter();
        // stateMachine.Animator.Play(MoveHash); // Begin moving immediately
        stateMachine.Animator.CrossFadeInFixedTime(MoveHash, CrossFadeDuration);
    }
}
