using UnityEngine;

/// <summary>
/// Celebrate state for the Bunny FSM using singletons
/// This has been replaced with the AgentLinkMover
/// Author: Geoffrey Roth
/// </summary>
public sealed class BunnyCelebrateState : BunnyBaseState {
    static readonly BunnyCelebrateState instance = new BunnyCelebrateState();
    public static BunnyCelebrateState Instance {
        get {
            return instance;
        }
    }
    static BunnyCelebrateState() {}
    private BunnyCelebrateState() {}

    public override void Enter(Bunny b)
    {
        // Debug.Log("Enter BunnyCelebrateState");
        base.Enter(b);
        b.anim.speed = 1.4f;
        SwitchAnimState(b, (int)Bunny.BunnyAnimState.CELEBRATE);
    }

    public override void Execute(Bunny b)
    {

    }

    public override void Exit(Bunny b)
    {

    }
}
