using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireController : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;
    public float distanceFromKitty;

    public GameObject player;
    public float bounceForce = 10f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
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

    private void ResetAnimator()
    {
        anim.Play("Normal");
    }
    
    private void OnTriggerEnter(Collider c)
    {
        
        if (c.gameObject.CompareTag("Player"))
        {
            Debug.Log("BOUNCING WITH KITTY");
            anim.Play("Bounce");
            c.GetComponent<ForceReceiver>().verticalVelocity = bounceForce;
            
            Invoke("ResetAnimator", anim.GetCurrentAnimatorStateInfo(0).length);
        }
        
    }
    
}
