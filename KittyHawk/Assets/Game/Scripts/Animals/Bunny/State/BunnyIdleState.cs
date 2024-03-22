using UnityEngine;

/// <summary>
/// Bunny Idle state
/// Author: Geoffrey Roth
/// </summary>
public sealed class BunnyIdleState : BunnyBaseState
{
    private const float THRESHOLD = 1.25f;
    static readonly BunnyIdleState instance = new BunnyIdleState();
    public static BunnyIdleState Instance
    {
        get {
            return instance;
        }
    }

    static BunnyIdleState() {}
    private BunnyIdleState() {}

    public override void Enter(Bunny b)
    {
        Debug.Log("Enter BunnyIdleState");
        base.Enter(b);
        SwitchAnimState(b, (int)Bunny.BunnyAnimState.IDLE);
        b.anim.speed = 1.4f;
        b.UpdateAgent = false;
        b.anim.SetFloat(VelocityXHash, 0);
        b.anim.SetFloat(VelocityZHash, 0);
        b.anim.SetFloat(MagnitudeHash, 0);
    }

    public override void Execute(Bunny b)
    {
        if (b.followMode && TargetHorizontalDistance(b) >= 2 * THRESHOLD)
        {
            b.agent.enabled = true;
            b.UpdateAgent = true;
            b.ChangeState(BunnyFollowState.Instance);
        }
    }

    public override void Exit(Bunny b) {
        Debug.Log("Exit BunnyIdleState");
    }
}
