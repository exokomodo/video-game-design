/*
 * GooseAI.cs
 * Authors: Paul Garza, James Orson (edits on goose death)
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
    protected NavMeshAgent agent;
    protected Animator anim;
    [SerializeField] private AIState aiState;
    private Rigidbody rb;
    private Collider cl;

    // Patrol state
    [SerializeField] private float wanderRadius; // Maximum allowable distance the Goose can walk when patrolling
    [SerializeField] private float chanceToWalk; // Random chance
    [SerializeField] protected float timeUntilNextWalk = 5f; // Time between walks
    [SerializeField] protected float timeSinceLastWalk = 0f; // Time since the Goose last walked
    [SerializeField] private Vector3 randomDirection;
    private NavMeshHit hit;

    // Flee state
    public GameObject kitty;
    private Transform kittyTransform;
    private Vector3 fleeDirection;
    [SerializeField] private bool isNearKitty;

    // All states
    public bool IsAlive => isAlive;
    [SerializeField] private bool isAlive = true;
    private Vector3 currentPosition;
    [SerializeField] protected Vector3 newPosition;

    private float HonkTimer;
    private float HonkTime;

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

        agent.updateRotation = false;

        // Initializing variables needed by states
        wanderRadius = 4f;

        // Starts the navigation process
        EnterPatrolState();
        ResetHonkTimer();
    }

    private void OnAttackEvent(string eventType, float attackTime, Collider c)
    {
        Debug.Log("OnAttackEvent > " + eventType);
        if (c != cl) return;
        if (eventType == AttackEvent.ATTACK_TARGET_HIT)
        {
            Debug.Log("ATTACK_TARGET_HIT > A goose has been hit by Kitty!");
            // TODO: Subtract health and potentially enter die state?
            EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, "GooseHit1");
            EnterFleeState();
        }
    }

    // Turns off everything for the Goose to save resources and to stop it from
    public void Die()
    {
        // Turns everything off and reduces Goose velocity
        isAlive = false;
        agent.ResetPath();
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;

        anim.SetBool("isWalking", false);
        anim.Play("Idle");

        cl.isTrigger = true;
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && isAlive && aiState == AIState.ATTACK)
        {
            // Run away from kitty after attacking. Coward.
            EventManager.TriggerEvent<AttackEvent, string, float, Collider>(AttackEvent.ATTACK_KITTY_HIT, 0f, other.collider);
            EnterFleeState();
        }
    }

    // These two functions help determine the flee state
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Goose sees Kitty");
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
            Debug.Log("Goose Entering Attack State");
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
        HonkTimer += Time.fixedDeltaTime;
        if (HonkTimer >= HonkTime)
        {
            Honk();
            ResetHonkTimer();
        }
    }

    // Used for patrol state timer
    protected void ResetPatrolTimer()
    {
        timeSinceLastWalk = 0f;
        timeUntilNextWalk = Random.Range(3.0f, 10.0f);
    }

    private void ChangeLookDirection(Vector3 targetPos)
    {
        // Make the chicken look at the new position. Uses euler transformation because the model
        // is oriented the wrong way. +90 didn't work for some reason so -270 it is.
        Vector3 lookPosition = targetPos - currentPosition;
        Quaternion rotation = Quaternion.LookRotation(lookPosition);
        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y - 270, 0);
    }

    protected void SetGooseDestination(Vector3 targetPos)
    {
        newPosition.y = transform.position.y;
        ChangeLookDirection(targetPos);
        agent.SetDestination(targetPos);
    }

    #region Patrol State
    protected virtual void EnterPatrolState()
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

    protected virtual void UpdatePatrolState()
    {
        // Update the time since last walk
        timeSinceLastWalk += Time.deltaTime;

        // Check if it's time to try walking again
        if (agent.pathPending == false && agent.remainingDistance < 0.5f)
        {
            // Sets value for animator to switch back to idle
            anim.SetBool("isWalking", false);
            agent.ResetPath();

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

                SetGooseDestination(newPosition);
                ResetPatrolTimer();
            }
        }
    }
    #endregion
    #region Flee State
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

            SetGooseDestination(newPosition);

            // When the flee destination is reached, will patrol if kitty isn't around
            if (!isNearKitty) EnterPatrolState();
        }
        // As soon as kitty is away enters patrol
        //if (!isNearKitty) EnterPatrolState();
    }
    #endregion
    #region Attack State
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
        Honk();
    }

    private void UpdateAttackState()
    {
        // Chases after kitty until she is caught or escapes
        newPosition = kittyTransform.position;
        SetGooseDestination(newPosition);

        // On collision has call for EnterFleeState()

        if (!isNearKitty) EnterPatrolState();
    }

    private void ResetHonkTimer()
    {
        HonkTimer = 0;
        HonkTime = Random.Range(15, 45);
    }

    public void Honk()
    {
        EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, $"GooseHonk{Random.Range(1, 3)}");
    }
    #endregion
}
