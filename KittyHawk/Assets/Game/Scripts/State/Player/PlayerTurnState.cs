using UnityEngine;

/// <summary>
/// The Kitty Turn State for when she abruptly changes directions
/// Author: Geoffrey Roth
/// </summary>
public class PlayerTurnState : PlayerBaseState
{
    public enum Turn {
        RIGHT180,
        LEFT180
    }
    private readonly int TurnTypeHash = Animator.StringToHash("TurnType");
    private Turn turnType;

    public PlayerTurnState(PlayerStateMachine stateMachine, Turn turnType) : base(stateMachine) {
        StateID = (int)PlayerStateMachine.StateEnum.TURN;
        this.turnType = turnType;
    }

    public override void Enter()
    {
        Debug.Log($"PlayerTurnState Enter > Turn: {turnType}, {(int)turnType}");
        EventManager.StartListening<AnimationStateEvent, AnimationStateEventBehavior.AnimationEventType, string>(OnAnimationEvent);
        stateMachine.Animator.SetInteger(TurnTypeHash, (int)turnType);
    }

    public override void Execute(float deltaTime)
    {
        // stateMachine.Controller.Rotate(Quaternion.Euler(0, 180, 0));
        // stateMachine.Controller.Rotate(Quaternion.Euler(0, 180 * deltaTime, 0));
        // Rotate(Quaternion.Euler(0, 180, 0), deltaTime);
    }

    public override void Exit()
    {
        Debug.Log("PlayerTurnState Exit");
        EventManager.StopListening<AnimationStateEvent, AnimationStateEventBehavior.AnimationEventType, string>(OnAnimationEvent);
    }

    private void OnAnimationEvent(AnimationStateEventBehavior.AnimationEventType eventType, string eventName)
    {
        Debug.Log("AnimationEvent received " + eventType + ", " + eventName);
        if (eventName == AnimationStateEvent.TURN_COMPLETE) stateMachine.Controller.SwitchToMoveState();
    }
}
