using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeowAction : PlayerBaseAction
{
    private readonly string AnimPath = HeadLayer + ".HeadActions.Meow";

    public PlayerMeowAction(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        this.ActionID = 21;
    }

    public override void Enter()
    {
        Debug.Log("PlayerMeowAction Enter");
        stateMachine.Animator.Play(AnimPath);
        EventManager.TriggerEvent<AudioEvent, Vector3, string>(stateMachine.Controller.transform.position, "Meow");
    }

    public override void Execute(float deltaTime)
    {

    }

    public override void Exit()
    {
        Debug.Log("PlayerMeowAction Exit");
    }
}
