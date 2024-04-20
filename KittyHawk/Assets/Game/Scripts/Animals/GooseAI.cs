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
using System;
using UnityEngine.AI;
using Random = UnityEngine.Random;


[RequireComponent(typeof(NavMeshAgent))]

public class GooseAI : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Animator anim;
    [SerializeField] private AIState aiState;
    protected Rigidbody rb;
    protected Collider cl;
    private LayerMask StaticObstacle;

    // Patrol state
    [SerializeField] private float wanderRadius; // Maximum allowable distance the Goose can walk when patrolling
    [SerializeField] private float chanceToWalk; // Random chance
    [SerializeField] protected float timeUntilNextWalk = 5f; // Time between walks
    [SerializeField] protected float timeSinceLastWalk = 0f; // Time since the Goose last walked
    [SerializeField] private Vector3 randomDirection;

    private const float GOOSE_FLEE_STATE_SPEED = 2.5f;
    private float hitBuffer { get { return 0.2f * GOOSE_FLEE_STATE_SPEED; } } 
    private NavMeshHit hit;

    // Flee state
    public GameObject kitty;
    private Transform kittyTransform;
    private Vector3 fleeDirection;
    [SerializeField] protected bool isNearKitty;

    // All states
    private bool attackEnabled = true;
    public bool IsAlive => isAlive;
    public bool IsAttacking => aiState == AIState.ATTACK;
    [SerializeField] protected bool isAlive = true;
    private Vector3 currentPosition;
    [SerializeField] protected Vector3 newPosition;
    [SerializeField] protected Vector3 spawnPosition;
    [SerializeField] protected float FollowRadius = 7f;

    private float HonkTimer;
    private float HonkTime;
    private float AttackCoolDown = 0;
    private float AttackCoolDownTime = 7.5f;

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
        EventManager.StartListening<DialogueOpenEvent, Vector3, string>(OnDialogueOpen);
        EventManager.StartListening<DialogueCloseEvent, string>(OnDialogueClose);
        StaticObstacle = LayerMask.GetMask("static_obstacle");
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
        AttackCoolDown = AttackCoolDownTime;

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
        /*
        if (other.gameObject.CompareTag("Player") && isAlive && aiState == AIState.ATTACK)
        {
            // Run away from kitty after attacking. Coward.
            EventManager.TriggerEvent<AttackEvent, string, float, Collider>(AttackEvent.ATTACK_KITTY_HIT, 0f, other.collider);
            EnterFleeState();
        }
        */

        if (other.gameObject.CompareTag("ChickenCoop"))
        {
            anim.SetBool("isWalking", false);
            agent.ResetPath();
        }
    }

    // These two functions help determine the flee state
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Goose sees Kitty");
            isNearKitty = true;
        }
    }

    // Entering the attack trigger made this trigger exit
    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         Debug.Log("Goose is away from kitty");

    //     }
    // }

    void FixedUpdate()
    {
        if (!isAlive) return;
        currentPosition = transform.position;
        // Added to replace the OnTriggerExit
        if (isNearKitty) {
            float dist = Vector3.Distance(currentPosition, kitty.transform.position);
            isNearKitty = dist < FollowRadius;
        }
        AttackCoolDown += Time.fixedDeltaTime;

        // Debug.Log($"isNearKitty: {isNearKitty}, AttackCoolDown: {AttackCoolDown}");
        if (isNearKitty && aiState != AIState.ATTACK && aiState != AIState.FLEE && attackEnabled && AttackCoolDown > AttackCoolDownTime)
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
        // Make the goose look at the new position. Uses euler transformation because the model
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
        rb.rotation = Quaternion.identity;
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
    public void EnterFleeState()
    {
        Debug.Log("Goose Entering Flee State");
        AttackCoolDown = 0;
        agent.ResetPath();
        aiState = AIState.FLEE;
        rb.velocity = Vector3.zero;
        agent.speed = 2.5f;
        anim.SetBool("isAttacking", false);
        anim.SetBool("isWalking", true);
    }

    private RaycastHit? CheckCollisionHit()
    {
        Vector3 direction = transform.forward;
        Vector3 origin = transform.position;
        RaycastHit hitInfo;
        bool hit = RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(origin, direction, out hitInfo, 0.5f, StaticObstacle, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor);
        return hit? hitInfo : null;
    }



    private void UpdateFleeState()
    {
        if (isNearKitty)
        {
            // Finds the vector which points away from kitty, normalizes it,
            // places it at the end of the wander radius and sets it as the new destination
            fleeDirection = currentPosition - kittyTransform.position;
            fleeDirection = fleeDirection.normalized;
            newPosition = (fleeDirection * wanderRadius) + currentPosition;

            
            // Stays in the Navmesh
            if (NavMesh.Raycast(currentPosition, randomDirection, out hit, NavMesh.AllAreas))
            {
                  newPosition = hit.position;
            }

            RaycastHit? hitInfo = CheckCollisionHit();
            if (hitInfo != null) 
            {
                RaycastHit hit = (RaycastHit)hitInfo;
                Vector3 loc = hit.collider.ClosestPointOnBounds(transform.position);
                Vector3 dist = transform.position - loc;
                // Debug.LogWarning($"STOP: {loc}, {dist}");
                if (Math.Abs(dist.x) < hitBuffer) 
                {
                    newPosition.x = currentPosition.x;
                }
                if (Math.Abs(dist.z) < hitBuffer) 
                {
                    newPosition.z = currentPosition.z;
                }
            }

            SetGooseDestination(newPosition);

            // When the flee destination is reached, will patrol if kitty isn't around
            //if (!isNearKitty) EnterPatrolState();
        }
        // As soon as kitty is away enters patrol
        if (!isNearKitty) {
            Debug.Log($"UpdateFleeState > {isNearKitty}");
            EnterPatrolState();
        }
    }
    #endregion
    #region Attack State
    private void EnterAttackState()
    {
        // Plays small jump-like animation to let the player know that the goose is going to attack
        Debug.Log("Goose Entering Attack State");
        agent.ResetPath();
        rb.velocity = Vector3.zero;
        agent.speed = GOOSE_FLEE_STATE_SPEED;
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

    void OnDialogueOpen(Vector3 point, string arg) {
        attackEnabled = false;
        if (aiState != AIState.PATROL) EnterPatrolState();
    }

    void OnDialogueClose(string arg) {
        attackEnabled = true;
    }

    void OnDestroy() {
        EventManager.StopListening<AttackEvent, string, float, Collider>(OnAttackEvent);
        EventManager.StopListening<DialogueOpenEvent, Vector3, string>(OnDialogueOpen);
        EventManager.StopListening<DialogueCloseEvent, string>(OnDialogueClose);
    }
}
