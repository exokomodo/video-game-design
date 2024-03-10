using UnityEngine;

/// <summary>
/// The Kitty Die State
/// Corresponding animations are found in the "Base Layer" of the Animation Controller.
/// Author: Geoffrey Roth
/// </summary>
public class PlayerDieState : PlayerMoveBase
{
    private Vector3 momentum;

    public PlayerDieState(PlayerStateMachine stateMachine) : base(stateMachine) {
        StateID = (int)PlayerStateMachine.StateEnum.DIE;
    }

    public override void Enter()
    {
        Debug.Log("PlayerDieState Enter");
        // Store player momentum at the time they enter the Die state
        momentum = stateMachine.Controller.velocity;
        momentum.y = 0;
        string hitSound = $"CatHit{Random.Range(1, 4)}";
        EventManager.TriggerEvent<AudioEvent, Vector3, string>(stateMachine.Controller.transform.position, hitSound);
    }

    public override void Execute(float deltaTime)
    {
        // Add player's previous momentum so they don't Die straight down
        AddForce(momentum * deltaTime, ForceMode.Acceleration);
    }

    public override void Exit()
    {
        Debug.Log("PLAYER_DIE_STATE Exit");
    }
}
