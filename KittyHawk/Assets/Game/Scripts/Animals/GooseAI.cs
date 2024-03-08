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

public class GooseAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    [SerializeField] private AIState aiState;
    private Rigidbody rb;
    private Collider cl;

    // Patrol state
    [SerializeField] private float wanderRadius; // Maximum allowable distance the Goose can walk when patrolling
    [SerializeField] private float chanceToWalk; // Random chance 
    [SerializeField] private float timeUntilNextWalk = 5f; // Time between walks
    [SerializeField] private float timeSinceLastWalk = 0f; // Time since the Goose last walked
    [SerializeField] private Vector3 randomDirection;
    private NavMeshHit hit;

    // Flee state
    public GameObject kitty;
    private Transform kittyTransform;
    private Vector3 fleeDirection;
    [SerializeField] private bool isNearKitty;

    // All states
    [SerializeField] private bool isAlive = true;
    private Vector3 currentPosition;
    [SerializeField] private Vector3 newPosition;
    
    public enum AIState
    {
        PATROL,
        FLEE,
        ATTACK
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        cl = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        kittyTransform = kitty.GetComponent<Transform>();
        
        // I'll handle this. 
        agent.updateRotation = false;

        // Initializing variables needed by states
        wanderRadius = 4f;
        
        // Starts the navigation process
        EnterPatrolState();

    }

    // Turns off everything for the Goose to save resources and to stop it from 
    private void Die()
    {
        // Turns everything off and reduces Goose velocity
        isAlive = false;
        //AIAgent.enabled = false;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;

        anim.SetBool("isWalking", false);
        anim.Play("Idle");

        cl.isTrigger = true;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && isAlive)
        {
            //TODO: Kitty has to take damage
            // Run away from kitty after attacking. Coward.
            EnterFleeState();
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

        if (isNearKitty && aiState != AIState.ATTACK && aiState != AIState.FLEE)
        {
            EnterAttackState();
        }

        switch (aiState)
        {
            // Patrol state: Move around the navmesh randomly
            case AIState.PATROL:
                UnityEngine.Debug.Log("Inside UpdatePatrolState");
                UpdatePatrolState();
                break;

            case AIState.FLEE:
                UpdateFleeState();
                break;
            
            case AIState.ATTACK:
                UpdateAttackState();
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
        UnityEngine.Debug.Log("Changing look direction");
        // Make the Goose look at the new position. Uses euler transformation because the model 
        // is oriented the wrong way. +90 didn't work for some reason so -270 it is.
        // TODO: Quaternion.Slerp?
        Vector3 lookPosition = newPosition - currentPosition;
        Quaternion rotation = Quaternion.LookRotation(lookPosition);
        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y - 270, 0);
    }
    
    private void EnterPatrolState()
    {
        UnityEngine.Debug.Log("Goose Entering Patrol State");
        agent.ResetPath();
        aiState = AIState.PATROL;
        rb.velocity = Vector3.zero;
        agent.speed = 0.6f;
        anim.SetBool("isWalking", false);
        anim.SetBool("isAttacking", false);
        ResetPatrolTimer();
    }

    private void UpdatePatrolState()
    {
        currentPosition = transform.position;
        
        // Update the time since last walk
        timeSinceLastWalk += Time.deltaTime;

        // Check if it's time to try walking again
        if (agent.pathPending == false && agent.remainingDistance < 0.5f)
        {
            UnityEngine.Debug.Log("Should be walking!");
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

                // We don't want the Goose walking around where it can't get to
                newPosition.y = transform.position.y;
                
                ChangeLookDirection();
                
                // Sets the destination for the AI Goose
                agent.SetDestination(newPosition);

                ResetPatrolTimer();
            }
        }
    }

    private void EnterFleeState()
    {
        UnityEngine.Debug.Log("Goose Entering Flee State");
        agent.ResetPath();
        rb.velocity = Vector3.zero;
        agent.speed = 2.5f;
        aiState = AIState.FLEE;
        anim.SetBool("isAttacking", false);
        anim.SetBool("isWalking", true);
        

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
            
            // When the flee destination is reached, will patrol if kitty isn't around
            if (!isNearKitty) EnterPatrolState();
        }
        // As soon as kitty is away enters patrol
        //if (!isNearKitty) EnterPatrolState();
    }

    private void EnterAttackState()
    {
        // Plays small jump-like animation to let the player know that the goose is going to attack
        UnityEngine.Debug.Log("Goose Entering Attack State");
        agent.ResetPath();
        rb.velocity = Vector3.zero;
        agent.speed = 2.5f;
        aiState = AIState.ATTACK;      
        
        anim.SetBool("isAttacking", true);
        anim.SetBool("isWalking", true);
        anim.Play("StartAttack");
        
        //TODO: audio event for mean quack
    }

    private void UpdateAttackState()
    {
            // Chases after kitty until she is caught or escapes
            newPosition = kittyTransform.position;
            ChangeLookDirection();
            agent.SetDestination(newPosition);
        
            // On collision has call for EnterFleeState()
            
            if (!isNearKitty) EnterPatrolState();
    }
}