/*
 * TireController.cs
 * Authors: Paul Garza, Geoffrey Roth
 * Date: 03/09/24
 * Summary: This script serves as the control for bouncing tires in the game
 * 
 * Planned updates: This script can still be tweaked some to accommodate more natural and predictable movement
 *
 * Dependencies: Player components.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireController : MonoBehaviour
{
    private Animator tireAnim;
    public GameObject player;

    [SerializeField] public bool AbsoluteValueOn;
    private Rigidbody playerRb;
    private PlayerController playerController;

    [SerializeField] private float bounceForce = 1f;
    [SerializeField] private Vector3 approachDirection;

    // Start is called before the first frame update
    void Start()
    {
        AbsoluteValueOn = true;
        tireAnim = GetComponent<Animator>();
        playerRb = player.GetComponent<Rigidbody>();
        playerController = player.GetComponent<PlayerController>();
    }

    // Method to set the animator back to its original state after the tire bounces.
    // Needed to make sure the animation plays before kitty can bounce on it again.
    private void ResetAnimator()
    {
        tireAnim.Play("Normal");
    }

    private void EnterFallState()
    {
        playerController.SwitchToFallState();
    }

    private void OnCollisionEnter(Collision c)
    {
        // When kitty collides with the tire stack, animation plays, kitty gets thrown in the air
        if (c.gameObject.CompareTag("Player") && tireAnim.GetCurrentAnimatorStateInfo(0).length > 0)
        {
            tireAnim.Play("Bounce");

            // Calculate the direction kitty hits the tire stack from
            approachDirection = player.transform.position - transform.position;
            approachDirection.y = 0;
            approachDirection = approachDirection.normalized;

            if (AbsoluteValueOn)
            {
                playerRb.velocity = new Vector3(Math.Abs(approachDirection.x) * 3f, bounceForce, Math.Abs(approachDirection.z) * 3f);
            }
            else
            {
                playerRb.velocity = new Vector3(approachDirection.x * -2.0f, bounceForce, approachDirection.z * -2.0f);
            }

            // Normally we expect to Kitty to fall from some height, but here she's typically grounded.
            // We delay entering the fall state to give the kitty time to acquire some height
            // so she doesn't automatically go into the landing state.
            Invoke("EnterFallState", 0.2f);

            // ensures the bounce finishes before the animator is reset by waiting length of the Bounce animation
            Invoke("ResetAnimator", tireAnim.GetCurrentAnimatorStateInfo(0).length);

            EventManager.TriggerEvent<TireStackBounceEvent, Vector3>(transform.position);
        }
    }
}
