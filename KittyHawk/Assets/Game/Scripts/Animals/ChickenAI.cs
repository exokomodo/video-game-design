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
    
    // Patrol state
    private float wanderRadius; // Maximum allowable distance the chicken can walk when patrolling
    private float chanceToWalk; // Random chance 
    private float timeUntilNextWalk = 5f; // Time between walks
    private float timeSinceLastWalk = 0f; // Time since the chicken last walked
    private Vector3 randomDirection;
    private NavMeshHit hit;
    
    // Flee state
    public GameObject kitty;
    private Transform kittyTransform;
    public bool isNearKitty;
    
    // All states
    private bool isAlive = true;
    private Vector3 currentPosition;
    public Vector3 newPosition;
    

    
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
        anim = GetComponent<Animator>();
        kittyTransform = kitty.GetComponent<Transform>();
        
        // I'll handle this. 
        agent.updateRotation = false;
            
        // Initializing variables needed by states
        wanderRadius = 4f;
        agent.speed = 0.6f;
    }

    private void IsNearKitty()
    {
        
        if (Vector3.Distance(kittyTransform.position, currentPosition)< 5.0f)
        { 
            UnityEngine.Debug.Log("hiKitty");
            isNearKitty = true;
        }
        
    }

    private void OnCollisionEnter(Collision other)
    {
        UnityEngine.Debug.Log("COLLISION CHICKEN");
         if (other.gameObject.CompareTag("ChickenCoop") && isAlive)
         {
             UnityEngine.Debug.Log("Goodbye Chicken");
             isAlive = false;
             agent.enabled = false;
             rb.isKinematic = true;
             rb.velocity = Vector3.zero;
             
             // TODO: Make it so all of the chickens don't spawn in the same place
             transform.position = other.transform.position;
             
             anim.SetBool("isWalking", false);
             anim.Play("Idle");
             
             this.GetComponent<CapsuleCollider>().isTrigger = true;

         }
    }

    void Update()
    {
        if (!isAlive) return;
        
        switch (aiState)
        {
            // Patrol state: Move around the navmesh randomly
            case AIState.patrolState:
                currentPosition = transform.position;
                IsNearKitty();
                
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
                        
                        // Make the chicken look at the new position. Uses euler transformation because the model 
                        // is oriented the wrong way. +90 didn't work for some reason so -270 it is.
                        Vector3 lookPosition = newPosition - currentPosition;
                        Quaternion rotation = Quaternion.LookRotation(lookPosition);
                        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y -270, 0);
                        
                        // Sets the destination for the AI chicken
                        agent.SetDestination(newPosition);

                        // Reset the timer until the next walk and randomly sets length until next walk
                        timeUntilNextWalk = Random.Range(3.0f, 10.0f);
                        timeSinceLastWalk = 0f; // Reset the timer
                    }
                }
                break;
                
        }
    }
}
