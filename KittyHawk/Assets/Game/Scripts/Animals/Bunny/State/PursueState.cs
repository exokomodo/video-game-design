using UnityEngine;
using UnityEngine.AI;

public sealed class PursueState :  BunnyBaseState {

  private float timeToTarget;
  private GameObject target;
  private VelocityReporter velo;
  private const float THRESHOLD = 1.5f;

  private GameObject marker;

  static readonly PursueState instance = new PursueState();
  public static PursueState Instance {
    get {
      return instance;
    }
  }
  static PursueState() { }
  private PursueState() { }

  public override void Enter (Bunny m) {
    Debug.Log("Enter PursueState");
    target = m.CurrentWaypoint;
    m.anim.SetInteger("StateID", (int)Bunny.BunnyAnimState.MOVE);
    m.anim.SetTrigger("StateChanged");
    velo = target.GetComponent("VelocityReporter") as VelocityReporter;
    marker = GameObject.FindGameObjectsWithTag("destination")[0];
    marker.transform.position = target.transform.position;
    UpdateDestination(m);
  }

  public override void Execute(Bunny m)
  {
      m.anim.SetFloat("VelocityX", m.agent.velocity.x / m.agent.speed);
      m.anim.SetFloat("VelocityZ", m.agent.velocity.z / m.agent.speed);
      if (!m.agent.pathPending && TargetDistance(m) < THRESHOLD)
      {
          m.SetNextWaypoint();
      }
      else
      {
          UpdateDestination(m);
      }
  }

  private void UpdateDestination(Bunny m)
  {
    float agentSpeed = m.agent.speed;
    float dist = TargetDistance(m);
    timeToTarget = Mathf.Clamp(dist/agentSpeed, 0, 2.0f);
    Vector3 targetPos = ExtrapolatedTargetPosition(timeToTarget);

    NavMeshHit hit;
    bool blocked = NavMesh.Raycast(marker.transform.position, targetPos, out hit, NavMesh.AllAreas);
    if (!blocked)
    {
      marker.transform.position = targetPos;
      m.SetAgentDestination(marker.transform.position);
    }
  }

  public override void Exit(Bunny m) {
    Debug.Log("Exit PursueState");
  }

  private float TargetDistance(Bunny m)
  {
    return Vector3.Distance(m.transform.position, target.transform.position);
  }

  private Vector3 ExtrapolatedTargetPosition(float t)
  {
    return target.transform.position + velo.velocity * t;
  }
}
