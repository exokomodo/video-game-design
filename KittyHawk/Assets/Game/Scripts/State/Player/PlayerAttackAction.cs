using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackAction : PlayerBaseAction
{
    private readonly string AttackPath = ActionLayer + ".Attacks.";
    public static readonly int ATTACK_RIGHT = 11;
    public static readonly int ATTACK_FRONT = 12;
    public static readonly int ATTACK_LEFT = 13;

    public static readonly Dictionary<int, string> AttackType = new Dictionary<int, string>
    {
        {ATTACK_RIGHT, "AttackRight"},
        {ATTACK_FRONT, "AttackFront"},
        {ATTACK_LEFT, "AttackLeft"}
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
