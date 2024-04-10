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
}
