using UnityEngine;
using UnityEngine.AI;

public sealed class BunnyPursueState :  BunnyBaseState {

  private float timeToTarget;
  private VelocityReporter velo;
  private const float THRESHOLD = 1.5f;

  private GameObject marker;

  static readonly BunnyPursueState instance = new BunnyPursueState();
  public static BunnyPursueState Instance {
    get {
      return instance;
    }
  }
  static BunnyPursueState() { }
  private BunnyPursueState() { }

  public override void Enter (Bunny b) {
    Debug.Log("Enter BunnyPursueState");
    target = b.CurrentWaypoint;
    SwitchAnimState(b, (int)Bunny.BunnyAnimState.MOVE);
    velo = target.GetComponent("VelocityReporter") as VelocityReporter;
    marker = GameObject.FindGameObjectsWithTag("destination")[0];
    marker.transform.position = target.transform.position;
    UpdateDestination(b);
  }

  public override void Execute(Bunny b)
  {
      b.anim.SetFloat("VelocityX", b.agent.velocity.x / b.agent.speed);
      b.anim.SetFloat("VelocityZ", b.agent.velocity.z / b.agent.speed);
      if (!b.agent.pathPending && TargetDistance(b) < THRESHOLD)
      {
          b.SetNextWaypoint();
      }
      else
      {
          UpdateDestination(b);
      }
  }

  private void UpdateDestination(Bunny b)
  {
    float agentSpeed = b.agent.speed;
    float dist = TargetDistance(b);
    timeToTarget = Mathf.Clamp(dist/agentSpeed, 0, 2.0f);
    Vector3 targetPos = ExtrapolatedTargetPosition(timeToTarget);

    NavMeshHit hit;
    bool blocked = NavMesh.Raycast(marker.transform.position, targetPos, out hit, NavMesh.AllAreas);
    if (!blocked)
    {
      marker.transform.position = targetPos;
      b.SetAgentDestination(marker.transform.position);
    }
  }

  public override void Exit(Bunny b) {
    Debug.Log("Exit BunnyPursueState");
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
