using UnityEngine;

public sealed class BunnyPatrolState : BunnyBaseState {
    private const float THRESHOLD = 1.0f;
    static readonly BunnyPatrolState instance = new BunnyPatrolState();
    public static BunnyPatrolState Instance {
        get {
            return instance;
        }
    }
    static BunnyPatrolState() {}
    private BunnyPatrolState() {}

    public override void Enter(Bunny b) {
        b.SetAgentDestination();
        SwitchAnimState(b, (int)Bunny.BunnyAnimState.MOVE);
    }

    public override void Execute(Bunny b) {
        b.anim.SetFloat("VelocityX", b.agent.velocity.x / b.agent.speed);
        b.anim.SetFloat("VelocityZ", b.agent.velocity.z / b.agent.speed);
        if (!b.agent.pathPending && b.agent.remainingDistance < THRESHOLD)
        {
            b.SetNextWaypoint();
        }
    }

    public override void Exit(Bunny m) {
        Debug.Log("Leaving BunnyPatrolState");
    }
}
