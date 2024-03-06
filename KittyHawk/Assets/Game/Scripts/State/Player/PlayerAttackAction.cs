using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackAction : PlayerBaseAction
{
    private readonly string AttackPath = ActionLayer + ".Attacks.";

    public static readonly Dictionary<int, string> AttackType = new Dictionary<int, string>
    {
        {(int)PlayerStateMachine.ActionEnum.ATTACK_RIGHT, "AttackRight"},
        {(int)PlayerStateMachine.ActionEnum.ATTACK_FRONT, "AttackFront"},
        {(int)PlayerStateMachine.ActionEnum.ATTACK_LEFT, "AttackLeft"}
    };


    public PlayerAttackAction(PlayerStateMachine stateMachine, int attackType) : base(stateMachine)
    {
        this.ActionID = attackType;
        Debug.Log("ActionID: " + ActionID);
    }

    public override void Enter()
    {
        Debug.Log("PlayerAttackAction Enter");
        stateMachine.Animator.Play(AttackPath + AttackType[ActionID]);
    }

    public override void Execute(float deltaTime)
    {

    }

    public override void Exit()
    {
        Debug.Log("PlayerAttackAction Exit");
    }
}
