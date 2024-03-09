using UnityEngine;

/// <summary>
/// The Kitty Fall State
/// Corresponding animations are found in the "Base Layer" of the Animation Controller.
/// Author: Geoffrey Roth
/// </summary>
public class PlayerFallState : PlayerMoveBase
{
    private Vector3 momentum;
    private float elapsedTime;

    public PlayerFallState(PlayerStateMachine stateMachine) : base(stateMachine) {
        StateID = (int)PlayerStateMachine.StateEnum.FALL;
        elapsedTime = 0;
    }

    public override void Enter()
    {
        Debug.Log("PlayerFallState Enter");
        // Store player momentum at the time they enter the fall state
        momentum = stateMachine.Controller.velocity;
        momentum.y = 0;
    }

    public override void Execute(float deltaTime)
    {
        // Add player's previous momentum so they don't fall straight down
        AddForce(momentum * deltaTime, ForceMode.Acceleration);
        elapsedTime += deltaTime;
        if (elapsedTime > 3f)
        {
            // Sometimes KH gets stuck in the fall state
            // This will help her transition back to a moveable state
            EventManager.TriggerEvent<AnimationStateEvent, AnimationStateEventBehavior.AnimationEventType, string>(AnimationStateEventBehavior.AnimationEventType.TIME, AnimationStateEvent.LAND_COMPLETE);
        }
    }

    public override void Exit()
    {
        Debug.Log("PLAYER_FALL_STATE Exit");
        elapsedTime = 0;
    }
}
