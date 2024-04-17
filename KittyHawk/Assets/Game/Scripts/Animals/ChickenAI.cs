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
    [SerializeField] private AIState aiState;
    private Rigidbody rb;
    private Collider cl;

    private GameObject coopGroup;
    CoopGroupController coopGroupController;

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
    private Ray ray;

    private Vector3 spawnPosition;
    [SerializeField] private bool isNearKitty;

    // All states
    private bool isAlive = true;
    private Vector3 currentPosition;
    [SerializeField] private Vector3 newPosition;
    private GameObject gate;
    private RaycastHit rayHit;

    public enum AIState
    {
        PATROL,
        FLEE,
        INCOOP // Not used yet
    }

    void Start()
    {

        aiState = AIState.PATROL;
        spawnPosition = this.transform.position;
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        cl = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        kittyTransform = kitty.GetComponent<Transform>();
        gate = GameObject.FindWithTag("Gate");
        coopGroup = GameObject.FindWithTag("CoopGroup");
        coopGroupController = coopGroup.GetComponent<CoopGroupController>();
        agent.updateRotation = false;

        // Initializing variables needed by states
        wanderRadius = 4f;
        agent.speed = 0.6f;
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
            UnityEngine.Debug.Log("Chicken has collided with the coop!");
            if (coopGroupController.kittyNearCoops == true)
            {
                UnityEngine.Debug.Log("Chicken has been caught by Kitty!");
                Die();
                GameObject coop = other.gameObject;
                EnterInCoopState(coop);
            }
            else
            {
                SetChickenDestination(spawnPosition);
                UnityEngine.Debug.Log("Chicken has NOT entered the coop!");
            }
        }

        if (other.gameObject.CompareTag("Gate") && gate.GetComponent<GateController>().gateOpen == false)
        {
            agent.ResetPath();
            SetChickenDestination(spawnPosition);
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

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearKitty = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        UnityEngine.Debug.Log("Chicken has exited the trigger");
        if (other.CompareTag("Player"))
        {
            isNearKitty = false;
        }
    }

    void FixedUpdate()
    {
        UnityEngine.Debug.Log("isNearKitty" + isNearKitty);
        if (!isAlive) return;
        currentPosition = this.transform.position;
        if (isNearKitty && aiState != AIState.FLEE)
        {
            UnityEngine.Debug.Log("Chicken is entering flee state");
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

    #region movement functions
    private void ChangeLookDirection(Vector3 targetPos)
    {
        // Make the chicken look at the new position. Uses euler transformation because the model 
        // is oriented the wrong way. +90 didn't work for some reason so -270 it is.
        Vector3 lookPosition = targetPos - currentPosition;
        Quaternion rotation = Quaternion.LookRotation(lookPosition);
        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y - 270, 0);
    }

    private void SetChickenDestination(Vector3 targetPos)
    {
        targetPos.y = transform.position.y;
        agent.SetDestination(targetPos);
        ChangeLookDirection(targetPos);
        ResetPatrolTimer();
    }


    #endregion

    #region Patrol State
    private void EnterPatrolState()
    {
        UnityEngine.Debug.Log("Chicken Returning to Patrol State");
        aiState = AIState.PATROL;
        agent.ResetPath();
        ResetPatrolTimer();
        rb.velocity = Vector3.zero;
        agent.speed = 0.6f;
        anim.SetBool("isWalking", false);
    }

    private void UpdatePatrolState()
    {
        // Update the time since last walk
        timeSinceLastWalk += Time.deltaTime;

        // Check if it's time to try walking again
        if (agent.pathPending == false && agent.remainingDistance < 0.4f)
        {
            // Sets value for animator to switch back to idle
            agent.ResetPath();
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

                SetChickenDestination(newPosition);
            }
        }
    }

    // Used for patrol state timer
    private void ResetPatrolTimer()
    {
        timeSinceLastWalk = 0f;
        timeUntilNextWalk = Random.Range(3.0f, 10.0f);
    }

    #endregion
    #region Flee State
    private void EnterFleeState()
    {
        UnityEngine.Debug.Log("Chicken Entering Flee State");
        agent.ResetPath();
        aiState = AIState.FLEE;
        anim.Play("OnFleeEnter");
        anim.SetBool("isWalking", true);
        rb.velocity = Vector3.zero;
        agent.speed = 2.5f;
        aiState = AIState.FLEE;
    }

    private void UpdateFleeState()
    {
        UnityEngine.Debug.Log("Chicken is in flee state currently so this SHOULD BE WORKING");
        // Finds the vector which points away from kitty, normalizes it,
        // places it at the end of the wander radius and sets it as the new destination
        fleeDirection = currentPosition - kittyTransform.position;
        fleeDirection = fleeDirection.normalized;
        newPosition = (fleeDirection * wanderRadius) + currentPosition;
    
            if (isNearKitty)
            {
                UnityEngine.Debug.Log("Chicken is near kitty and will flee.");
                ray = new Ray(currentPosition, fleeDirection);
                
                if (Physics.Raycast(ray, out rayHit, KITTY_FLEE_DISTANCE)) 
                {
                    if (rayHit.collider.tag == "Fence")
                    {
                        UnityEngine.Debug.Log("Hit gate, rotating 90 degrees.");
                        // Rotate 90 degrees to the right
                        Quaternion rotationChange = Quaternion.Euler(0, 90, 0); // Adjusts only the y-axis
                        fleeDirection = rotationChange * fleeDirection;
                        newPosition = currentPosition + (fleeDirection * wanderRadius); // Recalculate the new position                }
                }
                // Stays within the navmesh
                if (NavMesh.Raycast(currentPosition, randomDirection, out hit, NavMesh.AllAreas))
                {
                    randomDirection = Random.insideUnitSphere * wanderRadius;
                    randomDirection += currentPosition;
                    newPosition = randomDirection;
                }

                
                }
            SetChickenDestination(newPosition);
            }
            else
            {
                UnityEngine.Debug.Log("Chicken is no longer near kitty and will be entering patrol state.");
                if (agent.remainingDistance < 0.2f) EnterPatrolState();
            }
    }
    

    #endregion
    #region InCoop State
    void EnterInCoopState(GameObject coop)
    {
        UnityEngine.Debug.Log("Chicken Entering InCoop State");
        aiState = AIState.INCOOP;
        rb.velocity = Vector3.zero;
        agent.speed = 0f;
        anim.SetBool("isWalking", false);

        transform.position = coop.transform.position;
    }

    void UpdateInCoopState()
    {

    }
    #endregion
}