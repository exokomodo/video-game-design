/*
 * GooseAI.cs
 * Authors: Paul Garza
 * Date: 03/09/24
 * Summary: This script serves as the AI control for geese  within the game.
 *  It utilizes a finite state machine to manage different states of behavior - primarily patrol, attack, and flee.
 *  Patrol state allows geese to wander within a defined radius randomly.
 *  Attack state is triggered when kitty comes within a certain proximity and ends when kitty is far away enough or...
 *  Flee state is triggered when the goose hits kitty
 *
 * Planned updates: This script anticipates future integration into a more general animal AI system
 *
 * Dependencies: NavMesh Component. Animator Component.
 *
 */

using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]

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

    void Awake()
    {
        cl = GetComponent<CapsuleCollider>();
        EventManager.StartListening<AttackEvent, string, float, Collider>(OnAttackEvent);
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        kittyTransform = kitty.GetComponent<Transform>();

        // I'll handle this.
        agent.updateRotation = false;

        // Initializing variables needed by states
        wanderRadius = 4f;

        // Starts the navigation process
        EnterPatrolState();

    }

    private void OnAttackEvent(string eventType, float attackTime, Collider c)
    {
        if (eventType == AttackEvent.ATTACK_TARGET_HIT && c == cl)
        {
            Debug.Log("A goose has been hit by Kitty!");
            // TODO: Subtract health and potentially enter die state?
            EnterFleeState();
        }
        else if (eventType == AttackEvent.ATTACK_WITH_HORSE && c == cl)
        {
            Debug.Log("A goose has been hit by horse!");
            Die();
        }
    }

    // Turns off everything for the Goose to save resources and to stop it from
    private void Die()
    {
        // Turns everything off and reduces Goose velocity
        isAlive = false;
        agent.ResetPath();
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
        currentPosition = transform.position;

        if (isNearKitty && aiState != AIState.ATTACK && aiState != AIState.FLEE)
        {
            EnterAttackState();
        }

        switch (aiState)
        {
            // Patrol state: Move around the navmesh randomly
            case AIState.PATROL:
                // Debug.Log("Inside UpdatePatrolState");
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
        // Make the Goose look at the new position. Uses euler transformation because the model
        // is oriented the wrong way. +90 didn't work for some reason so -270 it is.
        // TODO: Quaternion.Slerp?
        Vector3 lookPosition = newPosition - currentPosition;
        Quaternion rotation = Quaternion.LookRotation(lookPosition);
        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y - 270, 0);
    }

    private void EnterPatrolState()
    {
        Debug.Log("Goose Entering Patrol State");
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
        // Update the time since last walk
        timeSinceLastWalk += Time.deltaTime;

        // Check if it's time to try walking again
        if (agent.pathPending == false && agent.remainingDistance < 0.5f)
        {
            // Debug.Log("Should be walking!");
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
        Debug.Log("Goose Entering Flee State");
        agent.ResetPath();
        aiState = AIState.FLEE;
        rb.velocity = Vector3.zero;
        agent.speed = 2.5f;
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


            agent.SetDestination(newPosition);
            ChangeLookDirection();


            // When the flee destination is reached, will patrol if kitty isn't around
            if (!isNearKitty) EnterPatrolState();
        }
        // As soon as kitty is away enters patrol
        //if (!isNearKitty) EnterPatrolState();
    }

    private void EnterAttackState()
    {
        // Plays small jump-like animation to let the player know that the goose is going to attack
        Debug.Log("Goose Entering Attack State");
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
