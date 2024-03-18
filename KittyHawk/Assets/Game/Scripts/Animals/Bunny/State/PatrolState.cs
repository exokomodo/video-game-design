using System;
using UnityEngine;

public sealed class PatrolState : FSMState<Bunny> {
    private const float THRESHOLD = 1.0f;
    static readonly PatrolState instance = new PatrolState();
    public static PatrolState Instance {
        get {
            return instance;
        }
    }
    static PatrolState() { }
    private PatrolState() { }

    public override void Enter(Bunny m) {
        m.SetDestination();
        m.anim.SetInteger("StateID", (int)Bunny.BunnyAnimState.MOVE);
        m.anim.SetTrigger("StateChanged");
    }

    public override void Execute(Bunny m) {
        m.anim.SetFloat("VelocityX", m.agent.velocity.x / m.agent.speed);
        m.anim.SetFloat("VelocityZ", m.agent.velocity.z / m.agent.speed);
        if (!m.agent.pathPending && m.agent.remainingDistance < THRESHOLD)
        {
            m.SetNextWaypoint();
        }
    }

    public override void Exit(Bunny m) {
        Debug.Log("Leaving PatrolState");
    }
}
