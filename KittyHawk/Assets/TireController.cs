using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireController : MonoBehaviour
{
    private Animator anim;
    public float distanceFromKitty;
    public GameObject mainCamera;
    public GameObject player;
    public float bounceForce = 10f;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    
    private void DetermineDistanceFromKitty()
    {
        distanceFromKitty = Vector3.Distance(player.transform.position, transform.position);
    }

    void Update()
    {
        //DetermineDistanceFromKitty();
    }

    // Method to set the animator back to its original state after the 
    private void ResetAnimator()
    {
        anim.Play("Normal");
    }
    
    
    private void OnTriggerEnter(Collider c)
    {
        // When kitty collides with the tire stack, animation plays, kitty gets thrown in the air
        if (c.gameObject.CompareTag("Player"))
        {
            anim.Play("Bounce");

            // Add vertical velocity to kitty
            // TODO: Eventually this might be changed if we change how verticalVelocity works. 
            // TODO: Should it bounce kitty only upwards or reverse her overall velocity? Can a force be added that 
            // is the opposite force that she is walking in?
            c.GetComponent<ForceReceiver>().verticalVelocity = bounceForce;

            // TODO: Kitty needs to change her animation state to jumping
            
            // ensures the bounce finishes before the animator is reset by waiting length of the Bounce animation
            Invoke("ResetAnimator", anim.GetCurrentAnimatorStateInfo(0).length);
            
            // TODO: Main camera pans up? 
            EventManager.TriggerEvent<TireStackBounceEvent, Vector3>(transform.position);
        }
    }
    
}
