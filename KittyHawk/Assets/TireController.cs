using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireController : MonoBehaviour
{
    private Animator anim;
    private float velx;
    private float velz;
    public GameObject player;
    private Rigidbody rb;
    private float bounceForce = 10f;
    private Vector3 approachDirection;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = player.GetComponent<Rigidbody>();
    }

    // Method to set the animator back to its original state after the 
    private void ResetAnimator()
    {
        anim.Play("Normal");
    }

    private void Update()
    {
        // captures velocity to use for bounce
         velx = rb.velocity.x;
         velz = rb.velocity.z;
    }

    private void OnCollisionEnter(Collision c)
    {
        
        // When kitty collides with the tire stack, animation plays, kitty gets thrown in the air
        if (c.gameObject.CompareTag("Player") && anim.GetCurrentAnimatorStateInfo(0).length > 0)
        {
            anim.Play("Bounce");

            // Calculate the direction kitty hits the tire stack from
            approachDirection = c.transform.position - this.transform.position;
            approachDirection.y = 0;
            approachDirection = approachDirection.normalized; // Normalize to get direction only

            // Add vertical velocity to kitty and keeps her remaining velocity
            c.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(approachDirection.x * 2f , bounceForce, approachDirection.z * -2f); 
            
            // Ends jump animation so falling can begin and kitty can jump
            EventManager.TriggerEvent<AnimationStateEvent, AnimationStateEventBehavior.AnimationEventType, 
                string>(AnimationStateEventBehavior.AnimationEventType.TIME, AnimationStateEvent.JUMP_COMPLETE);

            // Changes aniamation to falling
            c.gameObject.GetComponent<Animator>().Play("Fall", 0, -1);

            // ensures the bounce finishes before the animator is reset by waiting length of the Bounce animation
            Invoke("ResetAnimator", anim.GetCurrentAnimatorStateInfo(0).length);
            
            // TODO: Main camera pans up? 
            EventManager.TriggerEvent<TireStackBounceEvent, Vector3>(transform.position);
        }
    }
    
}
