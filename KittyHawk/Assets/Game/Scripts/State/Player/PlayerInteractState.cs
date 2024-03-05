using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractState : PlayerBaseState
{
    protected readonly int MatchTargetHash = Animator.StringToHash("MatchTarget");

    public PlayerInteractState(PlayerStateMachine stateMachine, bool MatchTarget = false) : base(stateMachine) {
        this.StateID = 6;
        stateMachine.Animator.SetBool(MatchTargetHash, MatchTarget);
        Debug.Log("MatchTarget: " + MatchTarget);
    }

    public override void Enter()
    {
        Debug.Log("PlayerInteractState Enter");
    }

    public override void Execute(float deltaTime)
    {

    }

    public override void Exit()
    {
        Debug.Log("PlayerInteractState Exit");
    }
}
