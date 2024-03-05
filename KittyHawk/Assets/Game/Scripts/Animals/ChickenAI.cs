using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Debug = System.Diagnostics.Debug;
using NavMeshBuilder = UnityEditor.AI.NavMeshBuilder;
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
        patrolState,
        fleeState,
        carriedState
    }

    void Start()
    {
        aiState = AIState.patrolState;
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
            // TODO Right now chicken spins when colliding with kitty
            transform.position = other.transform.position;
        }
    }

    private void OnTriggerStay(Collider other)
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

        if (isNearKitty)
        {
            EnterFleeState();
            //TODO: Small jump flee animation for feedback to player
        }

        switch (aiState)
        {
            // Patrol state: Move around the navmesh randomly
            case AIState.patrolState:
                UpdatePatrolState();
                break;

            case AIState.fleeState:
                UpdateFleeState();
                break;
        }
    }
    
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
        aiState = AIState.patrolState;
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
        agent.ResetPath();
        aiState = AIState.fleeState;
        anim.SetBool("isWalking", true);
        rb.velocity = Vector3.zero;
        agent.speed = 1.5f;
    }

    private void UpdateFleeState()
    {
        // Get direction to run from kitty
        fleeDirection = currentPosition - kittyTransform.position;
        fleeDirection = fleeDirection.normalized * wanderRadius;
        newPosition = fleeDirection + currentPosition;
        
        ChangeLookDirection();

        if (agent.pathPending == false && agent.remainingDistance < 0.2f && !isNearKitty)
        {
            EnterPatrolState();
        }
        
        agent.SetDestination(newPosition);
    }
}