using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Hit Actions for Kitty
/// Corresponding animations are found in the "ActionsLayer" of the Animation Controller.
/// Author: Geoffrey Roth
/// </summary>
public class PlayerHitAction : PlayerBaseAction
{
    private readonly string AnimPath = ActionLayer + ".Hit.PlayerHit";
    private float stunnedDuration = 1.0f;
    private float stunnedTimer = 0;

    public PlayerHitAction(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        ActionID = (int)PlayerStateMachine.ActionEnum.HIT;
        // Debug.Log("ActionID: " + ActionID);
    }

    public override void Enter()
    {
        Debug.Log("PlayerHitAction Enter");
        int randInt = Random.Range(1, 4);
        stateMachine.Animator.Play(AnimPath + randInt);
        stateMachine.Controller.Disable();
        string hitSound = $"CatHit{Random.Range(1, 4)}";
        EventManager.TriggerEvent<AudioEvent, Vector3, string>(stateMachine.Controller.transform.position, hitSound);
    }

    public override void Execute(float deltaTime)
    {
        stunnedTimer += deltaTime;
        if (stunnedTimer >= stunnedDuration)
        {
            stateMachine.ActionComplete(this);
        }
    }

    public override void Exit()
    {
        Debug.Log("PlayerHitAction Exit");
        stateMachine.Controller.Enable();
    }
}
