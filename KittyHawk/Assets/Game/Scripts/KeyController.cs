/*
 * KeyController.cs
 * Authors: Paul Garza, James Orson
 * Date: 03/09/24
 * Summary: This script serves as the controller for a collectable key. Right now, collecting it will
 * open a gate on the farm. 
 *
 * Planned updates: This script can still be altered to apply to more than just the gate. 
 *
 * Dependencies: Something to open, int his case the gate. 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    private GameObject player;
    private GameObject gate;
    private float playerDistance;
    public float playerDistanceToActivate = 5.0f;
    private Animator animGate;
    private Animator animKey;
    private GateController gateController;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");

        animKey = GetComponent<Animator>();

        gate = GameObject.FindWithTag("Gate");
        gateController = gate.GetComponent<GateController>();
        animGate = gate.GetComponent<Animator>();
    }

    public bool DetermineNearby()
    {
        // uses a vector to determine how far the player is, if the player is close enough, return true
        playerDistance = Vector3.Distance(player.transform.position, transform.position);
        return playerDistance < playerDistanceToActivate;

    }

    void FixedUpdate()
    {
        // If the player is close enough, the key will rotate,
        // if the player leaves, the key returns to its original position
        if (DetermineNearby())
        {
            animKey.SetBool("RotateKey", true);
        }
        else
        {
            animKey.SetBool("RotateKey", false);
        }
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Player"))
        {
            gateController.setGateOpen();
            Destroy(this.gameObject);
        }
    }
}


