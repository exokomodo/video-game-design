using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerMoveBase
{
    private readonly int MoveHash = Animator.StringToHash("Move");

    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        this.StateID = 3;
    }

    public override void Enter()
    {
        Debug.Log("PlayerMoveState Enter");
        base.Enter();
        stateMachine.Animator.Play(MoveHash); // Begin moving immediately
    }
}
