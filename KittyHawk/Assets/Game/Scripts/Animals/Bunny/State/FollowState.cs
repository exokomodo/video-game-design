using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

public sealed class FollowState :  FSMState<Bunny> {

  private float timeToTarget;
  private GameObject target;
  private VelocityReporter velo;
  private const float THRESHOLD = 1.5f;

  static readonly FollowState instance = new FollowState();
  public static FollowState Instance
  {
    get {
      return instance;
    }
  }
  static FollowState() {}
  private FollowState() {}

  public override void Enter(Bunny b)
  {
    Debug.Log("Enter FollowState");
    target = b.followTarget;
    ChangeAnimState(b, (int)Bunny.BunnyAnimState.MOVE);
    velo = target.GetComponent("VelocityReporter") as VelocityReporter;
    UpdateDestination(b);
  }

  public override void Execute(Bunny b)
  {
    float dist = TargetDistance(b);
    if (!b.agent.pathPending && dist < THRESHOLD)
    {
        ChangeAnimState(b, (int)Bunny.BunnyAnimState.IDLE);
        b.agent.velocity = Vector3.zero;
        b.anim.speed = 1.4f;
        return;
    }
    else
    ChangeAnimState(b, (int)Bunny.BunnyAnimState.MOVE);
    UpdateDestination(b);

    float scaleFactor = b.agent.speed;// * 1/(float)Math.Pow(Math.E, dist);
    Debug.Log($"Update position, dist: {dist}, scaleFactor: {scaleFactor}");
    b.anim.SetFloat("VelocityX", b.agent.velocity.x / scaleFactor);
    b.anim.SetFloat("VelocityZ", b.agent.velocity.z / scaleFactor);
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
      b.SetDestination(target.transform.position);
    }
  }

  public override void Exit(Bunny m) {
    Debug.Log("Exit FollowState");
  }

  private void ChangeAnimState(Bunny b, int StateID)
  {
    int curAnimStateID = b.anim.GetInteger(StateIDHash);
    Debug.Log($"curAnimStateID: {curAnimStateID}, StateID: {StateID}");
    if (curAnimStateID == StateID) return;
    b.anim.SetInteger(StateIDHash, StateID);
    b.anim.SetTrigger(StateChangeDHash);
  }

  private float TargetDistance(Bunny b)
  {
    return Vector3.Distance(b.transform.position, target.transform.position);
  }

  private Vector3 ExtrapolatedTargetPosition(float t)
  {
    return target.transform.position + velo.velocity * t;
  }
}
