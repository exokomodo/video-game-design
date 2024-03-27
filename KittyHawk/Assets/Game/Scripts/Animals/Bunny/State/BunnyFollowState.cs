using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Follow state for the Bunny FSM using singletons
/// Bunny will follow the first GameObject with the FollowTarget component attached
/// Author: Geoffrey Roth
/// </summary>
public sealed class BunnyFollowState : BunnyBaseState {

  private float timeToTarget;
  private VelocityReporter velo;
  private const float THRESHOLD = 1.25f;

  static readonly BunnyFollowState instance = new BunnyFollowState();
  public static BunnyFollowState Instance
  {
    get {
      return instance;
    }
  }

  static BunnyFollowState() {}
  private BunnyFollowState() {}

  public override void Enter(Bunny b)
  {
    Debug.Log("Enter BunnyFollowState");
    base.Enter(b);
    b.UpdateAgent = true;
    SwitchAnimState(b, (int)Bunny.BunnyAnimState.MOVE);
    velo = target.GetComponent("VelocityReporter") as VelocityReporter;
    UpdateDestination(b);
  }

  public override void Execute(Bunny b)
  {
    if (b.agent.pathPending) return;

    // float horizDist = TargetHorizontalDistance(b);
    // float vertDist = TargetSignedVerticalDistance(b);

    // if (horizDist <= 2 * THRESHOLD && vertDist <= 0.05f)
    if (b.agent.remainingDistance <= THRESHOLD)
    {
        b.ChangeState(BunnyIdleState.Instance);
        return;
    }

    int animState = (int)Bunny.BunnyAnimState.MOVE;
    if (b.agent.isOnOffMeshLink)
    {
      animState = (int)Bunny.BunnyAnimState.JUMP;
      b.anim.speed = 0.5f;
      SwitchAnimState(b, animState);
      return;
    }
    // b.UpdateAgent = true;
    // int animState = b.agent.isOnOffMeshLink? (int)Bunny.BunnyAnimState.JUMP : (int)Bunny.BunnyAnimState.MOVE;
    SwitchAnimState(b, animState);
    UpdateDestination(b);

    // float dur = b.LinkMoveDuration;

    Debug.Log($"Agent Velocity: {b.agent.velocity}");
    b.anim.SetFloat(VelocityXHash, b.agent.velocity.x / b.agent.speed);
    b.anim.SetFloat(VelocityZHash, b.agent.velocity.z / b.agent.speed);
    b.anim.SetFloat(MagnitudeHash, b.agent.velocity.magnitude);
    b.anim.speed = 1.8f;
  }

  private void UpdateDestination(Bunny b)
  {
    float agentSpeed = b.agent.speed;
    float dist = TargetDistance(b);
    timeToTarget = Mathf.Clamp(dist/agentSpeed, 0, 2.0f);
    Vector3 targetPos = ExtrapolatedTargetPosition(timeToTarget);

    NavMeshHit hit;
    bool blocked = NavMesh.Raycast(target.transform.position, targetPos, out hit, NavMesh.AllAreas);
    if (!blocked && dist > 2 * THRESHOLD)
    {
      b.SetAgentDestination(target.transform.position);
    }
  }

  public override void Exit(Bunny m) {
    Debug.Log("Exit BunnyFollowState");
  }

  private float TargetDistance(Bunny b)
  {
    return Vector3.Distance(b.transform.position, target.transform.position);
  }

  private float TargetVerticalDistance(Bunny b)
  {
    return Math.Abs(TargetSignedVerticalDistance(b));
  }

  private float TargetSignedVerticalDistance(Bunny b)
  {
    return b.transform.position.y - target.transform.position.y;
  }

  private Vector3 ExtrapolatedTargetPosition(float t)
  {
    return target.transform.position + velo.velocity * t;
  }


}
