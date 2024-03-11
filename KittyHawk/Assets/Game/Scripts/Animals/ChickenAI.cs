/*
 * ChickenAI.cs
 * Authors: Paul Garza
 * Date: 03/09/24
 * Summary: This script serves as the AI control for chicks and chickens within the game.
 *  It utilizes a finite state machine to manage different states of behavior - primarily patrol and flee.
 *  Patrol state allows chicken to wander within a defined radius randomly.
 *  Flee state is triggered when kitty comes within a certain proximity
 *
 * Update 03/10/24: Fixed chicken movement to properly flee from kitty
 * 
 * Planned updates: This script anticipates future integration into a more general animal AI system
 * and an additional 'carried' state *
 *
 * Dependencies: NavMesh Component. Animator Component. Player components.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Debug = System.Diagnostics.Debug;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;
using Random = UnityEngine.Random;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class ChickenAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private AIState aiState;
    private Rigidbody rb;
    private Collider cl;

    // Patrol state
    [SerializeField] private float wanderRadius; // Maximum allowable distance the chicken can walk when patrolling
    [SerializeField] private float chanceToWalk; // Random chance 
    [SerializeField] private float timeUntilNextWalk = 5f; // Time between walks
    [SerializeField] private float timeSinceLastWalk = 0f; // Time since the chicken last walked
    [SerializeField] private Vector3 randomDirection;
    private NavMeshHit hit;

    // Flee state
    public GameObject kitty;
    private const float KITTY_FLEE_DISTANCE = 2.0f;
    private Transform kittyTransform;
    private Vector3 fleeDirection;
    [SerializeField] private bool isNearKitty;

    // All states
    private bool isAlive = true;
    private Vector3 currentPosition;
    [SerializeField] private Vector3 newPosition;

    
    public enum AIState
    {
        PATROL,
        FLEE,
        CARRIED // Not used yet
    }

    void Start()
    {
        aiState = AIState.PATROL;
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        cl = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        kittyTransform = kitty.GetComponent<Transform>();

        // I'll handle this. 
        agent.updateRotation = false;

        // Initializing variables needed by states
        wanderRadius = 4f;
        agent.speed = 0.6f;
    }

    private void DetermineKittyProximity()
    {
        UnityEngine.Debug.Log("KITTY NEARBY???");
        isNearKitty = Vector3.Distance(kittyTransform.position, currentPosition) < KITTY_FLEE_DISTANCE;
    }

    // Turns off everything for the chicken to save resources and to stop it from 
    private void Die()
    {
        // Turns everything off and reduces chicken velocity
        isAlive = false;
        agent.enabled = false;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;

        anim.SetBool("isWalking", false);
        anim.Play("Idle");

        cl.isTrigger = true;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("ChickenCoop") && isAlive)
        {
            Die();
            
            // TODO: Make it so all of the chickens don't spawn in the same place
            transform.position = other.transform.position;
        }
    }

    // These two functions help determine the flee state
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearKitty = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearKitty = false;
        }
    }

    void FixedUpdate()
    {
        if (!isAlive) return;
        currentPosition = this.transform.position;
        if (isNearKitty && aiState != AIState.FLEE)
        {
            EnterFleeState();
            //TODO: Small jump flee animation for feedback to player
        }

        switch (aiState)
        {
            // Patrol state: Move around the navmesh randomly
            case AIState.PATROL:
                UpdatePatrolState();
                break;

            case AIState.FLEE:
                UpdateFleeState();
                break;
        }
    }
    
    // Used for patrol state timer
    private void ResetPatrolTimer()
    {
        timeSinceLastWalk = 0f;
        timeUntilNextWalk = Random.Range(3.0f, 10.0f);
    }
    
    
    private void ChangeLookDirection()
    {
        // Make the chicken look at the new position. Uses euler transformation because the model 
        // is oriented the wrong way. +90 didn't work for some reason so -270 it is.
        Vector3 lookPosition = newPosition - currentPosition;
        Quaternion rotation = Quaternion.LookRotation(lookPosition);
        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y - 270, 0);
    }
    
    private void EnterPatrolState()
    {
        UnityEngine.Debug.Log("Chicken Returning to Patrol State");
        aiState = AIState.PATROL;
        ResetPatrolTimer();
        rb.velocity = Vector3.zero;
        agent.speed = 0.6f;
        anim.SetBool("isWalking", false);
    }

    private void UpdatePatrolState()
    {
        currentPosition = transform.position;

        // Update the time since last walk
        timeSinceLastWalk += Time.deltaTime;

        // Check if it's time to try walking again
        if (agent.pathPending == false && agent.remainingDistance < 0.2f)
        {
            // Sets value for animator to switch back to idle
            anim.SetBool("isWalking", false);

            // Checks if enough time has elapsed
            if (timeSinceLastWalk >= timeUntilNextWalk)
            {
                // Animation set to walking state
                anim.SetBool("isWalking", true);

                // Finding random direction
                randomDirection = Random.insideUnitSphere * wanderRadius;
                randomDirection += currentPosition;

                // Stays within the navmesh
                if (NavMesh.Raycast(currentPosition, randomDirection, out hit, NavMesh.AllAreas))
                {
                    newPosition = hit.position;
                }
                else
                {
                    newPosition = randomDirection;
                }

                // We don't want the chicken walking around where it can't get to
                newPosition.y = transform.position.y;

                ChangeLookDirection();
                
                // Sets the destination for the AI chicken
                agent.SetDestination(newPosition);

                ResetPatrolTimer();
            }
        }
    }

    private void EnterFleeState()
    {
        UnityEngine.Debug.Log("Chicken Entering Flee State");
        agent.ResetPath();
        aiState = AIState.FLEE;
        anim.SetBool("isWalking", true);
        rb.velocity = Vector3.zero;
        agent.speed = 2.5f;
    }

    private void UpdateFleeState()
    {
        if (agent.pathPending == false && agent.remainingDistance < 0.2f)
        {
            // Finds the vector which points away from kitty, normalizes it,
            // places it at the end of the wander radius and sets it as the new destination
            fleeDirection = currentPosition - kittyTransform.position;
            fleeDirection = fleeDirection.normalized;
            newPosition = (fleeDirection * wanderRadius) + currentPosition;
            
            ChangeLookDirection();
            
            agent.SetDestination(newPosition);
            
            if (!isNearKitty) EnterPatrolState();
        }
        
        
    }
}