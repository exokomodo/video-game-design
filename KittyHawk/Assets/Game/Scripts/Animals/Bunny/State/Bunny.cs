using System;
using UnityEngine;
using UnityEngine.AI;

// FSM adapted from https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/
public class Bunny : MonoBehaviour
{
  private int currWaypoint = 0;
  private FiniteStateMachine<Bunny> FSM;

  public NavMeshAgent agent;
  public Animator anim;
  public GameObject[] waypoints;
  public bool followMode = false;
  public GameObject followTarget;
  public enum BunnyAnimState
  {
    IDLE,
    MOVE,
    JUMP
  }

  public enum BunnyState
  {
    PATROL,
    PURSUE,
    FOLLOW
  }

  public void Awake()
  {
    FSM = new FiniteStateMachine<Bunny>();
    FSM.Configure(this, GetState());
  }

  public void ChangeState(FSMState<Bunny> e)
  {
    FSM.ChangeState(e);
  }

  public void Update()
  {
    FSM.Update();
  }

  public void SetNextWaypoint()
  {
      ++currWaypoint;
      if (currWaypoint >= waypoints.Length) currWaypoint = 0;
      ChangeState(GetState());
  }

  public void SetDestination(Vector3? targetPos = null)
  {
    Vector3 pos = targetPos == null? CurrentWaypoint.transform.position : (Vector3)targetPos;
    if (pos != null) agent.SetDestination(pos);
  }

  public GameObject CurrentWaypoint
  {
    get {
      try
      {
        return waypoints[currWaypoint];
      }
      catch (Exception e)
      {
          Debug.LogError("Error: Could not set NavMeshAgent Destination: " + e.Message);
      }
      return null;
    }
  }

  private FSMState<Bunny> GetState()
  {
    return followMode? FollowState.Instance : PatrolState.Instance;
  }
}
