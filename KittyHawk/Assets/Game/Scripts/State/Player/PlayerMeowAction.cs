using UnityEngine;

/// <summary>
/// Because kitties meow.
/// Animates her head via custom avatar mask in the additive HeadActions layer.
/// Author: Geoffrey Roth
/// </summary>
public class PlayerMeowAction : PlayerBaseAction
{
    private readonly string AnimPath = HeadLayer + ".HeadActions.Meow";

    public PlayerMeowAction(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        ActionID = (int)PlayerStateMachine.ActionEnum.MEOW;
        BlendingType = (int)PlayerStateMachine.BlendingType.ADDITIVE;
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
