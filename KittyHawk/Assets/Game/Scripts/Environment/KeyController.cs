using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    private GameObject player;
    private GameObject gate;
    private float playerDistance;
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

    // Pickup key and open gate.
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gateController.setGateOpen();
            Destroy(this.gameObject);
        }
           
    }

    // Animates key based on if kitty is in the sphere collider or not
    private void OnTriggerEnter(Collider c)
    {
        animKey.SetBool("RotateKey", true);
    }

    private void OnTriggerExit(Collider other)
    {
        animKey.SetBool("RotateKey", false);
    }
}


