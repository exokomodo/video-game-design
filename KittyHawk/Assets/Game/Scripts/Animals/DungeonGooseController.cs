using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Slight mods to Paul's GooseAI for the Dungeon
/// Author: Geoffrey Roth
/// </summary>
public class DungeonGooseController : GooseAI
{
    [SerializeField]
    public List<GameObject> Waypoints;
    public Room room;
    protected int currWaypoint = 0;

    public DungeonGooseController() : base() {
        Waypoints = new List<GameObject>();
    }

    protected override void EnterPatrolState()
    {
        base.EnterPatrolState();
        agent.speed = 1f;
    }

    protected override void UpdatePatrolState()
    {
        // Update the time since last walk
        timeSinceLastWalk += Time.fixedDeltaTime;

        // Check if it's time to try walking again
        if (agent.pathPending == false && agent.remainingDistance < 0.75f)
        {
            // Sets value for animator to switch back to idle
            anim.SetBool("isWalking", false);
            agent.ResetPath();

            // Checks if enough time has elapsed
            if (timeSinceLastWalk >= timeUntilNextWalk)
            {
                // Animation set to walking state
                anim.SetBool("isWalking", true);

                if (++currWaypoint >= Waypoints.Count) currWaypoint = 0;

                newPosition = Waypoints[currWaypoint].transform.position;

                SetGooseDestination(newPosition);
                ResetPatrolTimer();
            }
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && IsInRoom())
        {
            // Is kitty in the room where it happened?
            Debug.Log("Goose sees Kitty");
            isNearKitty = true;
        }
    }

    protected void OnTriggerStay(Collider other)
    {
        if (isNearKitty) return;
        if (other.CompareTag("Player") && IsInRoom())
        {
            Debug.Log("Goose sees Kitty");
            isNearKitty = true;
        }
    }

    protected bool IsInRoom() {
        // Debug.Log($"Room.IsInRoom(room, kitty): {room.IsInRoom(kitty)}");
        return room.IsInRoom(kitty);
    }

    public void Disable() {
        // Turns everything off and reduces Goose velocity
        isAlive = false;
        agent.ResetPath();
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;

        anim.SetBool("isWalking", false);
        anim.Play("Idle");

        cl.isTrigger = true;
    }
}
