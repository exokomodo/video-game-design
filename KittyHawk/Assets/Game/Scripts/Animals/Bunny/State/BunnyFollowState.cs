using System;
using UnityEngine;
using UnityEngine.AI;

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

    float horizDist = TargetHorizontalDistance(b);
    float vertDist = TargetVerticalDistance(b);

    if (horizDist <= 2 * THRESHOLD)
    {
        if (vertDist <= -0.1f)
        {
          Debug.Log($"vertDist: {vertDist}");
          b.ChangeState(BunnyJumpState.Instance);
        }
        else
        {
          // b.agent.velocity = Vector3.zero;
          // SwitchAnimState(b, (int)Bunny.BunnyAnimState.IDLE);
          // b.anim.speed = 1.4f;
          // b.anim.SetFloat(VelocityXHash, 0);
          // b.anim.SetFloat(VelocityZHash, 0);
          b.ChangeState(BunnyIdleState.Instance);
        }
        return;
    }

    b.UpdateAgent = true;
    SwitchAnimState(b, (int)Bunny.BunnyAnimState.MOVE);
    UpdateDestination(b);


    Debug.Log($"Update position, horizDist: {horizDist}, vertDist: {vertDist}");
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
    return b.transform.position.y - target.transform.position.y;
  }

  private Vector3 ExtrapolatedTargetPosition(float t)
  {
    return target.transform.position + velo.velocity * t;
  }


}
