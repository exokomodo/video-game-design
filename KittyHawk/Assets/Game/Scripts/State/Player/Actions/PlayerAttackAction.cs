using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Attack Actions for Kitty
/// Corresponding animations are found in the "ActionsLayer" of the Animation Controller.
/// Author: Geoffrey Roth
/// </summary>
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
        ActionID = attackType;
    }

    public override void Enter()
    {
        Debug.Log("PlayerAttackAction Enter");
        EventManager.StartListening<AttackEvent, string, float, Collider>(OnAttackEvent);
        stateMachine.Animator.Play(AttackPath + AttackType[ActionID]);
        string sound = $"CatAttack{Random.Range(1, 3)}";
        EventManager.TriggerEvent<AudioEvent, Vector3, string>(stateMachine.Controller.transform.position, sound);
    }

    public override void Execute(float deltaTime)
    {

    }

    public override void Exit()
    {
        // Debug.Log("PlayerAttackAction Exit");
        EventManager.StopListening<AttackEvent, string, float, Collider>(OnAttackEvent);
    }

    private void OnAttackEvent(string eventType, float attackTime, Collider c)
    {
        Debug.Log($"PlayerAttackAction OnAttackEvent: {eventType}");
        switch (eventType)
        {
            case AttackEvent.ATTACK_BEGIN:
                stateMachine.Controller.Attack(true);
                break;

            case AttackEvent.ATTACK_END:
                stateMachine.Controller.Attack(false);
                break;

            case AttackEvent.ATTACK_STATE_EXIT:
                stateMachine.ActionComplete(this);
                break;
        }
    }
}
